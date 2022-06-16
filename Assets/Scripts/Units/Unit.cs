using System;
using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Units
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField]
        private UnityEvent onSelected;

        [SerializeField]
        private UnityEvent onDeselected;

        [SerializeField]
        private UnitMovement unitMovement;

        [SerializeField]
        private Targeter targeter;

        [SerializeField]
        private Health health;

        [SerializeField]
        private int resourceCost = 10;

        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawned;

        public static event Action<Unit> AuthorityOnUnitSpawned;
        public static event Action<Unit> AuthorityOnUnitDespawned;

        public int GetCost()
        {
            return resourceCost;
        }

        public Targeter GetTargeter()
        {
            return targeter;
        }

        public UnitMovement GetUnitMovement()
        {
            return unitMovement;
        }

        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            ServerOnUnitSpawned?.Invoke(this);
            health.ServerOnDie += ServerHandleDie;
        }

        public override void OnStopServer()
        {
            base.OnStopClient();
            ServerOnUnitDespawned?.Invoke(this);
            health.ServerOnDie -= ServerHandleDie;
        }

        [Server]
        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion


        #region Client

        public void Select()
        {
            if (!hasAuthority)
            {
                return;
            }
            onSelected?.Invoke();
        }

        public void Deselect()
        {
            if (!hasAuthority)
            {
                return;
            }
            onDeselected?.Invoke();
        }

        #endregion

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            AuthorityOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            if (!hasAuthority)
            {
                return;
            }
            AuthorityOnUnitDespawned?.Invoke(this);
        }
    }
}
