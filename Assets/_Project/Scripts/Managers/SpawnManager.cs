using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BobIO
{
    public class SpawnManager : NetworkBehaviour
    {
        [SerializeField] private int _playerSpawnDelay;
        [SerializeField] private List<SpawnArea> _spawnAreasList = new List<SpawnArea>();

        void Start()
        {
            Init();
        }

        private void Init()
        {
            
        }

        public void RegisterPlayer(NetworkObjectReference playerReference, ServerRpcParams serverRpcParams = default)
        {
            //if (!IsHost)
            //    return;

            playerReference.TryGet(out NetworkObject playerNetworkObject);
            Player player = playerNetworkObject.GetComponent<Player>();

            player.PlayerDied += OnPlayerDied;

            if (!player.IsAlive)
            {
                StartCoroutine(RespawnPlayerWithDelayCoroutine(player, _playerSpawnDelay));
            }
        }

        private void OnPlayerDied(Player player) => StartCoroutine(RespawnPlayerWithDelayCoroutine(player, _playerSpawnDelay));

        private IEnumerator RespawnPlayerWithDelayCoroutine(Player player, int delay)
        {
            Debug.Log($"START RESPAWNING PLAYER {player}");

            NetworkObjectReference playerReference = new NetworkObjectReference(player.NetworkObject);

            SetPlayerActiveClientRpc(playerReference, false);

            yield return new WaitForSeconds(delay);

            SetPlayerActiveClientRpc(playerReference, true);

            //player.gameObject.SetActive(true);
            player.RespawnServerRpc(DefinePlayerNewSpawnPosition());
        }

        [ClientRpc]
        private void SetPlayerActiveClientRpc(NetworkObjectReference playerReference, bool isActive)
        {
            playerReference.TryGet(out var playerNetworkObject);
            playerNetworkObject.gameObject.SetActive(isActive);
        }

        private Vector2 DefinePlayerNewSpawnPosition()
        {
            if (_spawnAreasList.Count == 0)
                return Vector2.zero;

            Bounds randomAreaBounds = _spawnAreasList[Random.Range(0, _spawnAreasList.Count)].Collider.bounds;

            Vector2 randomPositionInBounds = new Vector2(
                Random.Range(randomAreaBounds.min.x, randomAreaBounds.max.x),
                Random.Range(randomAreaBounds.min.y, randomAreaBounds.max.y)
            );

            return randomPositionInBounds;
        }
    }
}