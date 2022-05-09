using System.Collections.Generic;
using Mirror;
using Units;

namespace Networking
{
    public class RTSPlayer : NetworkBehaviour
    {
        private List<Unit> myUnits = new List<Unit>();

        public List<Unit> GetUnits()
        {
            return myUnits;
        }

        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        }

        private void ServerHandleUnitDespawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            {
                return;
            }
            myUnits.Remove(unit);
        }

        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            {
                return;
            }
            myUnits.Add(unit);
        }
        #endregion

        #region Client

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!isClientOnly)
            {
                return;
            }
            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            if (!isClientOnly)
            {
                return;
            }
            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        }

        private void AuthorityHandleUnitSpawned(Unit unit)
        {
            if (!hasAuthority)
            {
                return;
            }
            myUnits.Add(unit);
        }

        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            if (!hasAuthority)
            {
                return;
            }
            myUnits.Remove(unit);
        }

        #endregion
    }
}
