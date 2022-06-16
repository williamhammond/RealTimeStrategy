using System;
using Mirror;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private int id = -1;

    [SerializeField]
    private int price = 100;

    [SerializeField]
    private GameObject buildingPreview;

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;

    public GameObject GetBuildingPreview()
    {
        return buildingPreview;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetId()
    {
        return id;
    }

    public int GetPrice()
    {
        return price;
    }

    #region Server

    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBuildingDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        AuthorityOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopAuthority()
    {
        AuthorityOnBuildingDespawned?.Invoke(this);
    }

    #endregion
}
