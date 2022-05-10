using Mirror;
using UnityEngine;

namespace Units
{
    public class UnitProjectile : NetworkBehaviour
    {
        [SerializeField]
        private Rigidbody rigidbody = null;

        [SerializeField]
        private float destroyAfterSeconds = 5f;

        [SerializeField]
        private float launchForce = 10f;

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfterSeconds);
        }

        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        private void Start()
        {
            rigidbody.velocity = transform.forward * launchForce;
        }

        [ServerCallback]
        void OnTriggerEnter(Collider co) => DestroySelf();
    }
}
