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
        private GameObject lobbyUI;

        [SerializeField]
        private Button startGameButton;

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

        // TODO make UI dynamic or add better player count configuration and guards
        private void ClientHandleInfoUpdated()
        {
            var players = (
                (RTSNetworkManager)NetworkManager.singleton
            ).Players;

            var playerIndex = 0;
            foreach (var pair in players)
            {
                playerNameTexts[playerIndex].text = pair.Value.GetDisplayName();
                playerIndex++;
            }

            for (var i = players.Count; i < playerNameTexts.Count; i++)
            {
                playerNameTexts[i].text = "Waiting For Player...";
            }

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
