using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace BobIO
{
    public class Player : NetworkBehaviour
    {
        public Action<Player> PlayerDied;
        public Action<Player> PlayerSpawned;

        [Header("References")]
        [SerializeField] private Weapon _weapon;
        [SerializeField] private GameObject _playerBody;

        [Header("Properties")]
        [SerializeField] private float _maxHp;
        [SerializeField] private float _regen;

        private PlayerMovement _playerMovement;
        private SpawnManager _spawnManager;

        private bool _isAbleToShoot = true;

        private NetworkVariable<bool> _isAlive = new NetworkVariable<bool>(false);
        private NetworkVariable<float> _currentHp = new NetworkVariable<float>(0);
        private NetworkVariable<FixedString32Bytes> _nickname = new NetworkVariable<FixedString32Bytes>();
        private NetworkVariable<int> _killCount = new NetworkVariable<int>(0);


        public string Nickname { get { return _nickname.Value.ToString(); } }
        public float CurrentHpNormalized { get { return _currentHp.Value / _maxHp; } }
        public int KillCount { get => _killCount.Value; }
        public int Score { get { return KillCount * 100; } }
        public bool IsAlive { get { return _isAlive.Value; } }
        public bool IsAbleToShoot { get => _isAbleToShoot; }




        void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _spawnManager = ProjectContext.Instance.SpawnManager;
        }

        private void Update()
        {
            Debug.Log($"Is {Nickname} is able to shoot: {IsAbleToShoot}");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner && IsLocalPlayer)
                ProjectContext.Instance.SetPlayerOwner(this);

            RespawnServerRpc();
            //NetworkObject.NetworkHide(OwnerClientId);
            //_spawnManager.RegisterPlayerServerRpc(this);
            _spawnManager.RegisterPlayer(new NetworkObjectReference(NetworkObject));

            

            _currentHp.OnValueChanged += OnCurrentHpChange;
        }

        private void OnCurrentHpChange(float previousValue, float newValue)
        {
            if (_currentHp.Value < 0)
            {
                DieServerRpc();
            }
        }

        public void Move(Vector2 direction)
        {
            _playerMovement.Move(direction);
        }

        public void RotatePlayerBodyToCursor(Vector2 direction)
        {
            float cursorDirectionAngle = -Vector2.SignedAngle(direction, Vector2.right);

            _playerBody.transform.rotation = Quaternion.Euler(0, 0, cursorDirectionAngle);
        }

        private void Regenerate()
        {
            if (_currentHp.Value < _maxHp)
            {
                
            }
        }

        public void Shoot()
        {
            if (_isAbleToShoot)
                _weapon.Shoot(this);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DieServerRpc()
        {
            _isAlive.Value = false;
            PlayerDied?.Invoke(this);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RespawnServerRpc()
        {
            _isAlive.Value = true;
            _currentHp.Value = _maxHp;

            PlayerSpawned?.Invoke(this);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RespawnServerRpc(Vector2 spawnPosition)
        {
            _isAlive.Value = true;
            _currentHp.Value = _maxHp;
            ResetPositionClientRpc(spawnPosition);

            PlayerSpawned?.Invoke(this);
        }

        [ClientRpc]
        private void ResetPositionClientRpc(Vector2 spawnPosition)
        {
            transform.position = spawnPosition;
        }

        public void TakeDamage(float damage, ulong attackerClientId)
        {
            CalculateDamageServerRpc(damage);

            if (!IsAlive)
            {
                Player playerAttacker = NetworkManager.ConnectedClients[attackerClientId].PlayerObject.GetComponent<Player>();

                playerAttacker.AddKillServerRpc();

                Debug.Log($"Player {name} (ObjectID: {NetworkObjectId}, ClientID {OwnerClientId}) " +
                          $"is killed by Player {playerAttacker.name} (ObjectID: {playerAttacker.NetworkObjectId}, ClientID {playerAttacker.OwnerClientId})");
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void CalculateDamageServerRpc(float damage)
        {
            _currentHp.Value -= damage;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetNicknameServerRpc(string newNickname)
        {
            _nickname.Value = newNickname;
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddKillServerRpc()
        {
            _killCount.Value++;
        }

        public void SetIsAbleToShoot(bool isAbleToShoot)
        {
            Debug.Log("SetIsAbleToShoot!!!!");
            _isAbleToShoot = isAbleToShoot;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<SpawnArea>(out SpawnArea spawnArea))
            {
                SetIsAbleToShoot(false);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<SpawnArea>(out SpawnArea spawnArea))
            {
                SetIsAbleToShoot(true);
            }
        }
    }
}