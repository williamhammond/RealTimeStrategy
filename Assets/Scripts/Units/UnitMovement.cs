using Buildings;
using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField]
        private NavMeshAgent agent;

        [SerializeField]
        private Targeter targeter;

        [SerializeField]
        private float chaseRange = 10f;

        #region Server

        public override void OnStartServer()
        {
            GameoverHandler.ServerOnGameOver += ServerHandleOnGameOver;
        }

        public override void OnStopServer()
        {
            GameoverHandler.ServerOnGameOver -= ServerHandleOnGameOver;
        }

        [Server]
        public void ServerMove(Vector3 moveTo)
        {
            targeter.ClearTarget();

            if (!NavMesh.SamplePosition(moveTo, out var hit, 1f, NavMesh.AllAreas))
            {
                return;
            }
            agent.SetDestination(hit.position);
        }

        [Server]
        private void ServerHandleOnGameOver()
        {
            agent.ResetPath();
        }

        [ServerCallback]
        private void Update()
        {
            if (targeter.GetTarget())
            {
                if (
                    (targeter.GetTarget().transform.position - transform.position).sqrMagnitude
                    > chaseRange * chaseRange
                )
                {
                    agent.SetDestination(targeter.GetTarget().transform.position);
                }
                else if (agent.hasPath)
                {
                    agent.ResetPath();
                }
                return;
            }
            if (agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.ResetPath();
            }
        }

        [Command]
        public void CmdMove(Vector3 moveTo)
        {
            ServerMove(moveTo);
        }
        #endregion
    }
}
