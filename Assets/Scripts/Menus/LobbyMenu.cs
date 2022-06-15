using System.Collections.Generic;
using Mirror;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menus
{
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject lobbyUI = null;

        [SerializeField]
        private Button startGameButton = null;

        [SerializeField]
        private List<TMP_Text> playerNameTexts;

        private void Start()
        {
            RTSNetworkManager.ClientOnConnected += HandleClientConnected;
            RTSPlayer.AuthorityOnPartyOwnerStateUpdated += HandlePartyOwnerStateUpdated;
            RTSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
        }

        private void OnDestroy()
        {
            RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
            RTSPlayer.AuthorityOnPartyOwnerStateUpdated -= HandlePartyOwnerStateUpdated;
            RTSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        }

        public void StartGame()
        {
            NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
        }

        private void ClientHandleInfoUpdated()
        {
            Dictionary<string, RTSPlayer> players = (
                (RTSNetworkManager)NetworkManager.singleton
            ).Players;

            // for (int i = 0; i < players.Count; i++)
            // {
            //     playerNameTexts[i].text = players[i].GetDisplayName();
            // }
            //
            // for (int i = players.Count; i < playerNameTexts.Length; i++)
            // {
            //     playerNameTexts[i].text = "Waiting For Player...";
            // }

            startGameButton.interactable = players.Count >= 2;
        }

        private void HandlePartyOwnerStateUpdated(bool state)
        {
            startGameButton.gameObject.SetActive(state);
        }

        private void HandleClientConnected()
        {
            lobbyUI.SetActive(true);
        }

        public void LeaveLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }

            SceneManager.LoadScene(0);
        }
    }
}
