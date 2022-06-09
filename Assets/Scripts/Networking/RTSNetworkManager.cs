using System;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField]
        private GameObject unitBasePrefab;

        [SerializeField]
        private GameoverHandler gameOverHandlerPrefab;

        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;

        public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

        private bool _isGameInProgress = false;

        #region Server

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            if (!_isGameInProgress)
            {
                return;
            }

            conn.Disconnect();
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
            Players.Remove(player);

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            Players.Clear();
            _isGameInProgress = false;
        }

        public void StartGame()
        {
            // if (Players.Count < 2)
            // {
            //     return;
            // }

            _isGameInProgress = true;

            ServerChangeScene("Scene_Map_Test");
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
            Players.Add(player);
            player.SetTeamColor(
                (
                    new Color(
                        UnityEngine.Random.Range(0f, 1f),
                        UnityEngine.Random.Range(0f, 1f),
                        UnityEngine.Random.Range(0f, 1f)
                    )
                )
            );

            player.SetPartyOwner(Players.Count == 1);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
            {
                GameoverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
                NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
                foreach (RTSPlayer player in Players)
                {
                    GameObject baseInstance = Instantiate(
                        unitBasePrefab,
                        GetStartPosition().position,
                        quaternion.identity
                    );
                    NetworkServer.Spawn(baseInstance, player.connectionToClient);
                }
            }
        }

        #endregion

        #region Client

        public override void OnClientConnect()
        {
            base.OnClientConnect();

            ClientOnConnected?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();

            ClientOnDisconnected?.Invoke();
        }

        public override void OnStopClient() { }

        #endregion
    }
}
