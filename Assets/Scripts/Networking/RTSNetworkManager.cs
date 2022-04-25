using Mirror;
using UnityEngine;

namespace Networking
{
    public class RTSNetworkManager : NetworkManager 
    {
        [SerializeField] private GameObject unitSpawnerPrefab;

        public  override  void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            
            GameObject unitSpawnerInstance = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }
    }
}
