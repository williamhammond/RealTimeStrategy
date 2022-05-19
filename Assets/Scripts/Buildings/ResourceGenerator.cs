using System;
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

        private float timer;
        private RTSPlayer player;

        public override void OnStartServer()
        {
            timer = interval;
            player = connectionToClient.identity.GetComponent<RTSPlayer>();

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
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                player.SetResources(player.GetResources() + resourcesPerInterval);

                timer += interval;
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
