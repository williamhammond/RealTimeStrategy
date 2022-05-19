using System;
using System.Collections.Generic;
using Mirror;
using Units;
using UnityEngine;

namespace Networking
{
    public class RTSPlayer : NetworkBehaviour
    {
        [SerializeField]
        private Building[] buildings = Array.Empty<Building>();

        public event Action<int> ClientOnResourcesUpdated;

        private List<Unit> myUnits = new List<Unit>();
        private List<Building> myBuildings = new List<Building>();

        [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
        private int resources = 500;

        public List<Unit> GetUnits()
        {
            return myUnits;
        }

        public List<Building> GetBuildings()
        {
            return myBuildings;
        }

        public int GetResources()
        {
            return resources;
        }

        #region Server

        [Server]
        public void SetResources(int newResources)
        {
            this.resources = newResources;
        }

        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

            Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;

            Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
        }

        [Command]
        public void CmdTryPlaceBuilding(int buildingId, Vector3 position)
        {
            Building buildingToPlace = null;
            foreach (Building building in buildings)
            {
                if (building.GetId() == buildingId)
                {
                    buildingToPlace = building;
                    break;
                }
            }

            if (!buildingToPlace)
            {
                return;
            }

            GameObject buildingInstance = Instantiate(
                buildingToPlace.gameObject,
                position,
                buildingToPlace.transform.rotation
            );

            NetworkServer.Spawn(buildingInstance, connectionToClient);
        }

        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            {
                return;
            }

            myUnits.Add(unit);
        }

        private void ServerHandleUnitDespawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            {
                return;
            }

            myUnits.Remove(unit);
        }

        private void ServerHandleBuildingSpawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            {
                return;
            }

            myBuildings.Add(building);
        }

        private void ServerHandleBuildingDespawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            {
                return;
            }

            myBuildings.Remove(building);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            base.OnStartClient();
            if (NetworkServer.active)
            {
                return;
            }

            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

            Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            if (!isClientOnly || !hasAuthority)
            {
                return;
            }

            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        }

        private void AuthorityHandleUnitSpawned(Unit unit)
        {
            myUnits.Add(unit);
        }

        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            myUnits.Remove(unit);
        }

        private void AuthorityHandleBuildingSpawned(Building building)
        {
            myBuildings.Add(building);
        }

        private void AuthorityHandleBuildingDespawned(Building building)
        {
            myBuildings.Remove(building);
        }

        private void ClientHandleResourcesUpdated(int oldResources, int newResources)
        {
            ClientOnResourcesUpdated?.Invoke(newResources);
        }

        #endregion
    }
}
