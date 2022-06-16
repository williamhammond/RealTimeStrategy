using Mirror;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject landingPagePanel;

        [SerializeField]
        private TMP_InputField addressInput;

        [SerializeField]
        private Button joinButton;

        public void OnEnable()
        {
            RTSNetworkManager.ClientOnConnected += HandleClientConnected;
            RTSNetworkManager.ClientOnConnected += HandleClientDisconnected;
        }

        public void OnDisable()
        {
            RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
            RTSNetworkManager.ClientOnConnected -= HandleClientDisconnected;
        }

        public void Join()
        {
            var address = addressInput.text;

            NetworkManager.singleton.networkAddress = address;
            NetworkManager.singleton.StartClient();

            joinButton.interactable = false;
        }

        private void HandleClientConnected()
        {
            joinButton.interactable = true;

            landingPagePanel.SetActive(false);
            gameObject.SetActive(false);
        }

        private void HandleClientDisconnected()
        {
            joinButton.interactable = true;
        }
    }
}
