using Combat;
using Mirror;
using UnityEngine;

namespace Units
{
    public class UnitFiring : NetworkBehaviour
    {
        [SerializeField]
        private Targeter targeter;

        [SerializeField]
        private GameObject projectilePrefab;

        [SerializeField]
        private Transform projectileSpawnPoint;

        [SerializeField]
        private float fireRange = 5f;

        [SerializeField]
        private float fireRate = 1f;

        [SerializeField]
        private float rotationSpeed = 20f;

        private float lastFireTime;

        [ServerCallback]
        private void Update()
        {
            if (!targeter.GetTarget())
            {
                return;
            }
            if (!CanFireAtTarget())
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(
                targeter.GetTarget().transform.position - transform.position
            );

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            if (Time.time > (1 / fireRate) + lastFireTime)
            {
                var projectilePosition = projectileSpawnPoint.position;
                Quaternion projectileRotation = Quaternion.LookRotation(
                    targeter.GetTarget().GetAimAtPoint().position - projectilePosition
                );
                GameObject projectileInstance = Instantiate(
                    projectilePrefab,
                    projectilePosition,
                    projectileRotation
                );
                NetworkServer.Spawn(projectileInstance, connectionToClient);
                lastFireTime = Time.time;
            }
        }

        [Server]
        private bool CanFireAtTarget()
        {
            return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude
                <= fireRange * fireRange;
        }
    }
}
