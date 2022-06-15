using System;
using System.Collections.Generic;
using Mirror;
using Units;
using UnityEngine;

namespace Networking
{
    public class RTSPlayer : NetworkBehaviour
    {
        [SyncVar]
        private string uuid;

        [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
        private string displayName;

        [SerializeField]
        private Transform cameraTransform;

        [SerializeField]
        private Building[] buildings = Array.Empty<Building>();

        [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
        private int resources = 500;

        [SerializeField]
        private LayerMask buildBlockLayer = new LayerMask();

        [SerializeField]
        private float buildingRangeLimit = 5f;

        public event Action<int> ClientOnResourcesUpdated;

        public static event Action ClientOnInfoUpdated;
        public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

        [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
        private bool isPartyOwner = false;

        private Color teamColor = new Color();
        private readonly List<Unit> myUnits = new List<Unit>();
        private readonly List<Building> myBuildings = new List<Building>();

        public string GetUUID()
        {
            return uuid;
        }

        public string GetDisplayName()
        {
            return displayName;
        }

        public bool GetIsPartyOwner()
        {
            return isPartyOwner;
        }

        public Transform GetCameraTransform()
        {
            return cameraTransform;
        }

        public List<Unit> GetUnits()
        {
            return myUnits;
        }

        public List<Building> GetBuildings()
        {
            return myBuildings;
        }

        public Color GetTeamColor()
        {
            return teamColor;
        }

        public int GetResources()
        {
            return resources;
        }

        public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 position)
        {
            bool isOverlapping = Physics.CheckBox(
                position + buildingCollider.center,
                buildingCollider.size / 2,
                Quaternion.identity,
                buildBlockLayer
            );

            bool inRange = false;
            foreach (Building building in myBuildings)
            {
                if (
                    (position - building.transform.position).sqrMagnitude
                    <= buildingRangeLimit * buildingRangeLimit
                )
                {
                    inRange = true;
                    break;
                }
            }

            return !isOverlapping && inRange;
        }

        #region Server

        [Server]
        public void SetDisplayName(string newDisplayName)
        {
            displayName = newDisplayName;
        }

        [Server]
        public void SetPartyOwner(bool state)
        {
            isPartyOwner = state;
        }

        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

            Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;

            uuid = Guid.NewGuid().ToString();

            DontDestroyOnLoad(gameObject);
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;

            Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
        }

        [Server]
        public void SetResources(int newResources)
        {
            this.resources = newResources;
        }

        [Server]
        public void SetTeamColor(Color newTeamColor)
        {
            teamColor = newTeamColor;
        }

        [Command]
        public void CmdStartGame()
        {
            if (!isPartyOwner)
            {
                return;
            }

            ((RTSNetworkManager)NetworkManager.singleton).StartGame();
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

            if (resources < buildingToPlace.GetPrice())
            {
                return;
            }

            BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();
            if (!CanPlaceBuilding(buildingCollider, position))
            {
                return;
            }

            GameObject buildingInstance = Instantiate(
                buildingToPlace.gameObject,
                position,
                buildingToPlace.transform.rotation
            );

            NetworkServer.Spawn(buildingInstance, connectionToClient);
            SetResources(resources - buildingToPlace.GetPrice());
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

        private void ClientHandleDisplayNameUpdated(string oldName, string newName)
        {
            ClientOnInfoUpdated?.Invoke();
        }

        public override void OnStartAuthority()
        {
            if (NetworkServer.active)
            {
                return;
            }

            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

            Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
        }

        public override void OnStartClient()
        {
            if (NetworkServer.active)
            {
                return;
            }

            DontDestroyOnLoad(gameObject);
            ((RTSNetworkManager)NetworkManager.singleton).Players.Add(GetUUID(), this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            ClientOnInfoUpdated?.Invoke();
            if (!isClientOnly)
            {
                return;
            }

            ((RTSNetworkManager)NetworkManager.singleton).Players.Remove(GetUUID());

            if (!hasAuthority)
            {
                return;
            }

            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        }

        private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
        {
            if (!hasAuthority)
            {
                return;
            }

            AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
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
