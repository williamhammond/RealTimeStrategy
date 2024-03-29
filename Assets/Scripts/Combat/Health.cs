using System;
using Buildings;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class Health : NetworkBehaviour
    {
        [SerializeField]
        private int maxHealth;

        [SyncVar(hook = nameof(HandleHealthUpdated))]
        private int _currentHealth;

        public event Action ServerOnDie;

        public event Action<int, int> ClientOnHealthUpdated;

        #region Server
        public override void OnStartServer()
        {
            _currentHealth = maxHealth;
            UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
        }

        public override void OnStopServer()
        {
            UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
        }

        [Server]
        public void DealDamage(int amount)
        {
            if (_currentHealth == 0)
            {
                return;
            }

            _currentHealth = Mathf.Max(_currentHealth - amount, 0);

            if (_currentHealth == 0)
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
