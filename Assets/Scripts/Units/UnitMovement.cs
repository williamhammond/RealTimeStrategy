using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField]
    private NavMeshAgent agent = null;

    #region Server
    [Command]
    public void CmdMove(Vector3 moveTo)
    {
        if (!NavMesh.SamplePosition(moveTo, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            return;
        }
        agent.SetDestination(hit.position);
    }
    #endregion
}
