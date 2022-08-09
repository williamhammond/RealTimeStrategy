using Combat;
using Mirror;
using Networking;
using UnityEngine;

namespace Buildings
{
    public class ResourceGenerator : NetworkBehaviour
    {
        [SerializeField]
        private Health health;

        [SerializeField]
        private int resourcesPerInterval = 10;

        [SerializeField]
        private float interval = 1f;

        private float _timer;
        private RTSPlayer _player;

        public override void OnStartServer()
        {
            _timer = interval;
            _player = connectionToClient.identity.GetComponent<RTSPlayer>();

            health.ServerOnDie += ServerHandleDie;
            GameoverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleDie;
            GameoverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [Server]
        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                _player.SetResources(_player.GetResources() + resourcesPerInterval);

                _timer += interval;
            }
        }

        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        private void ServerHandleGameOver()
        {
            enabled = false;
        }
    }
}
