using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField]
    private NavMeshAgent agent = null;

    [SerializeField]
    private Targeter targeter = null;

    [SerializeField]
    private float chaseRange = 10f;

    #region Server

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
        targeter.ClearTarget();

        if (!NavMesh.SamplePosition(moveTo, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            return;
        }
        agent.SetDestination(hit.position);
    }
    #endregion
}
