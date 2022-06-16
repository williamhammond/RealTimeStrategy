using Buildings;
using Mirror;
using TMPro;
using UnityEngine;

namespace Menus
{
    public class GameOverDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text winnerNameText;

        [SerializeField]
        private GameObject gameOverDisplayParent;

        private void Start()
        {
            GameoverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void OnDestroy()
        {
            GameoverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        public void LeaveGame()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }
        }

        private void ClientHandleGameOver(string winner)
        {
            winnerNameText.text = $"{winner} Has Won!";

            gameOverDisplayParent.SetActive(true);
        }
    }
}
