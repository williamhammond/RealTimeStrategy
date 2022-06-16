using System;
using System.Collections.Generic;
using Mirror;

namespace Buildings
{
    public class GameoverHandler : NetworkBehaviour
    {
        private readonly List<UnitBase> bases = new();

        public static event Action<string> ClientOnGameOver;
        public static event Action ServerOnGameOver;

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
            bases.Add(unitBase);
        }

        [Server]
        private void ServerHandleBaseDespawned(UnitBase unitBase)
        {
            bases.Remove(unitBase);
            if (bases.Count < 2)
            {
                int playerId = bases[0].connectionToClient.connectionId;
                RpcGameOver($"Player {playerId}");
                ServerOnGameOver?.Invoke();
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
}
