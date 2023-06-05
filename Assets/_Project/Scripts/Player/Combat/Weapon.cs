using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BobIO
{
    public class Weapon : NetworkBehaviour
    {
        [SerializeField] private Projectile _projectile;
        [SerializeField] private GameObject _projectileSpawnPoint;
        [SerializeField] private float _cooldownDuration;

        private Cooldown _cooldown;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _cooldown = new Cooldown(_cooldownDuration);
        }

        public void Shoot(Player playerAttacker)
        {
            if (_cooldown.Enabled)
                return;

            SpawnProjectileServerRpc();

            //if (!IsServer)
            //{
            //    Debug.Log($"SHOOT by clientID: {OwnerClientId}");

            //    Projectile projectile = Instantiate(_projectile, _projectileSpawnPoint.transform.position, Quaternion.identity);
            //    projectile.ProjectileOwnerId = OwnerClientId;
            //    projectile.Launch(transform.right);
            //}
            

            _cooldown.Start();
        }

        [ServerRpc]
        private void SpawnProjectileServerRpc(ServerRpcParams serverRpcParams = default)
        {
            //var clientId = serverRpcParams.Receive.SenderClientId;

            //if (NetworkManager.ConnectedClients.ContainsKey(clientId))
            //{
            //    var client = NetworkManager.ConnectedClients[clientId];

            //    Projectile projectile = Instantiate(_projectile, _projectileSpawnPoint.transform.position, Quaternion.identity);
            //    projectile.GetComponent<NetworkObject>().Spawn(true);

            //    projectile.ProjectileOwnerId = clientId;
            //    //projectile.ProjectileOwnerId = client.PlayerObject.NetworkObjectId;
            //    projectile.Launch(transform.right);
            //}

            var clientId = serverRpcParams.Receive.SenderClientId;

            Projectile projectile = Instantiate(_projectile, _projectileSpawnPoint.transform.position, Quaternion.identity);
            projectile.GetComponent<NetworkObject>().Spawn(true);

            //if (!NetworkManager.ConnectedClientsList[(int)clientId].PlayerObject.IsOwnedByServer)
            //{
            //    projectile.NetworkObject.NetworkHide(clientId);
            //}


            //ClientRpcParams clientRpcParams = new ClientRpcParams
            //{
            //    Send = new ClientRpcSendParams
            //    {
            //        TargetClientIds = new ulong[] { clientId }
            //    }
            //};
            //DisableProjectileClientRpc(clientRpcParams)

            projectile.ProjectileOwnerId = clientId;
            projectile.Launch(transform.right);
        }

        //[ClientRpc]
        //private void SpawnProjectileClientRpc()
        //{
        //    if (!IsOwner && !IsLocalPlayer)
        //        return;


        //}

        //[ClientRpc]
        //private void DisableProjectileClientRpc(ClientRpcParams clientRpcParams)
        //{
        //    //if (!IsOwner && !IsLocalPlayer)
        //    //    return;

        //    //projectile.NetworkObject.NetworkHide(clientId);

        //}
    }
}