using System;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class Health : NetworkBehaviour
    {
        [SerializeField]
        private int maxHealth;

        [SyncVar(hook = nameof(HandleHealthUpdated))]
        private int currentHealth;

        public event Action ServerOnDie;

        public event Action<int, int> ClientOnHealthUpdated;

        #region Server
        public override void OnStartServer()
        {
            currentHealth = maxHealth;
            UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
        }

        public override void OnStopServer()
        {
            UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
        }

        [Server]
        public void DealDamage(int amount)
        {
            if (currentHealth == 0)
            {
                return;
            }

            currentHealth = Mathf.Max(currentHealth - amount, 0);

            if (currentHealth == 0)
            {
                ServerOnDie?.Invoke();
                Debug.Log("We died");
            }
        }

        [Server]
        private void ServerHandlePlayerDie(int connectionId)
        {
            if (connectionToClient.connectionId == connectionId)
            {
                DealDamage(maxHealth);
            }
        }

        #endregion

        #region Client

        private void HandleHealthUpdated(int oldHealth, int newHealth)
        {
            ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
        }

        #endregion
    }
}
