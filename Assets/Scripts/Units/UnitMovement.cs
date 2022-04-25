using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UnitMovement : NetworkBehaviour 
{
    [SerializeField] private NavMeshAgent agent = null;
    
    private Camera _mainCamera;

    #region Server
    [Command] 
    private void CmdMove(Vector3 moveTo)
    {
        if (!NavMesh.SamplePosition(moveTo, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            return;
        }
        agent.SetDestination(hit.position);
    }
    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        _mainCamera = Camera.main;
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority || !Mouse.current.rightButton.wasPressedThisFrame)
        {
            return;
        }
        
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            return;
        }
        
        CmdMove(hit.point);
    }

    #endregion
}
