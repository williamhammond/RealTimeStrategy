using System;
using Combat;
using Mirror;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField]
    private Health health;

    public static event Action<int> ServerOnPlayerDie;
    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;

    #region Server

    public override void OnStartServer()
    {
        ServerOnBaseSpawned?.Invoke(this);
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        ServerOnBaseDespawned?.Invoke(this);
        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client



    #endregion
}
