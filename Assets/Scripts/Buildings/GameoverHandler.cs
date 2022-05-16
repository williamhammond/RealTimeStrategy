using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameoverHandler : NetworkBehaviour
{
    private List<UnitBase> _bases = new List<UnitBase>();

    #region Server
    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        _bases.Add(unitBase);
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        _bases.Remove(unitBase);
        if (_bases.Count < 2)
        {
            Debug.Log("Game over");
        }
    }

    #endregion

    #region Client

    #endregion
}
