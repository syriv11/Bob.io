using Unity.Netcode;
using UnityEngine;

namespace BobIO
{
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private float _damage;
        [SerializeField] private float _projectileLifetime;
        [SerializeField] private float _projectileSpeed;

        public ulong ProjectileOwnerId { get; set; }
        public float Damage { get { return _damage; } }

        private void Awake()
        {
            Destroy(gameObject, _projectileLifetime);
        }

        public override void OnDestroy()
        {
            // Do explosion effect or something...

            DespawnProjectileServerRpc();

            Debug.Log("Projectile Destroyed!");
        }

        [ServerRpc(RequireOwnership = false)]
        private void DespawnProjectileServerRpc(ServerRpcParams serverRpcParams = default)
        {
            //if (ProjectileOwnerId.HasValue)
            //    NetworkObject.Despawn();

            NetworkObject.Despawn();
        }

        public void Launch(Vector2 direction)
        {
            GetComponent<Rigidbody2D>().AddForce(direction * _projectileSpeed, ForceMode2D.Impulse);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsServer)
                return;

            //if (!ProjectileOwnerId.HasValue)
            //    return;

            if (collision.gameObject.TryGetComponent<Player>(out Player player))
            {
                if (player.OwnerClientId != ProjectileOwnerId)
                {
                    player.TakeDamage(_damage, ProjectileOwnerId);
                }
                else
                {
                    Debug.Log("Collided with Owner");
                    return;
                }
            }

            if (collision.gameObject.TryGetComponent<Projectile>(out Projectile projectile))
            {
                if (projectile.OwnerClientId != ProjectileOwnerId)
                {
                    // Collided with enemy projectile!
                }
                else
                {
                    return;
                }
            }

            Destroy(gameObject);
        }
    }
}