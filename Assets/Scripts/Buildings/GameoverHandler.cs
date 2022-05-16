using System;
using System.Collections.Generic;
using Mirror;
using Networking;
using UnityEngine;

public class GameoverHandler : NetworkBehaviour
{
    private readonly List<UnitBase> _bases = new List<UnitBase>();

    public static event Action<string> ClientOnGameOver;

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
            int playerId = _bases[0].connectionToClient.connectionId;
            RpcGameOver($"Player {playerId}");
        }
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
