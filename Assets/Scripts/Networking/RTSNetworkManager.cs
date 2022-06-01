using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField]
        private GameObject unitSpawnerPrefab;

        [SerializeField]
        private GameoverHandler gameOverHandlerPrefab;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
            player.SetTeamColor(
                (
                    new Color(
                        UnityEngine.Random.Range(0f, 1f),
                        UnityEngine.Random.Range(0f, 1f),
                        UnityEngine.Random.Range(0f, 1f)
                    )
                )
            );

            GameObject unitSpawnerInstance = Instantiate(
                unitSpawnerPrefab,
                conn.identity.transform.position,
                conn.identity.transform.rotation
            );
            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
            {
                GameoverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
                NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
            }
        }
    }
}
