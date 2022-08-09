using Mirror;
using Steamworks;
using UnityEngine;

namespace Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject landingPagePanel;

        [SerializeField]
        private bool useSteam;

        private Callback<LobbyCreated_t> _lobbyCreated;
        private Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequested;
        private Callback<LobbyEnter_t> _lobbyEntered;

        private void Start()
        {
            if (useSteam && SteamManager.Initialized)
            {
                _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
                _gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(
                    OnGameLobbyJoinRequested
                );
                _lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            }
        }

        public void HostLobby()
        {
            landingPagePanel.SetActive(false);

            if (useSteam && SteamManager.Initialized)
            {
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
                return;
            }

            NetworkManager.singleton.StartHost();
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                landingPagePanel.SetActive(true);
                return;
            }
            NetworkManager.singleton.StartHost();

            SteamMatchmaking.SetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                "HostAddress",
                SteamUser.GetSteamID().ToString()
            );
        }

        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            if (NetworkServer.active)
            {
                return;
            }
            var hostAddress = SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                "HostAddress"
            );
            NetworkManager.singleton.networkAddress = hostAddress;
            NetworkManager.singleton.StartClient();

            landingPagePanel.SetActive(false);
        }
    }
}
