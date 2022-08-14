using System;
using System.Collections.Generic;
using Buildings;
using Mirror;
using Steamworks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField]
        private GameObject unitBasePrefab;

        [SerializeField]
        private bool useSteam;

        [SerializeField]
        private GameoverHandler gameOverHandlerPrefab;

        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;

        public Dictionary<string, RTSPlayer> Players { get; } = new();

        private bool _isGameInProgress;

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
            var player = conn.identity.GetComponent<RTSPlayer>();
            Players.Remove(player.GetUUID());

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            Players.Clear();
            _isGameInProgress = false;
        }

        public void StartGame()
        {
            if (Players.Count < 2)
            {
                return;
            }

            _isGameInProgress = true;

            ServerChangeScene("Scene_Map_Test");
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            var player = conn.identity.GetComponent<RTSPlayer>();
            Players.Add(player.GetUUID(), player);
            if (useSteam && SteamManager.Initialized)
            {
                if (NetworkServer.active)
                {
                    player.SetDisplayName(SteamFriends.GetPersonaName());
                }
            }
            else
            {
                player.SetDisplayName($"Player {Players.Count}");
            }

            player.SetTeamColor(
                new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f))
            );

            player.SetPartyOwner(Players.Count == 1);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
            {
                var gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
                NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
                foreach (var player in Players.Values)
                {
                    var baseInstance = Instantiate(
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
