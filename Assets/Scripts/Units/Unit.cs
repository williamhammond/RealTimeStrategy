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
        private UnityEvent onSelected = null;

        [SerializeField]
        private UnityEvent onDeselected = null;

        [SerializeField]
        private UnitMovement unitMovement = null;

        [SerializeField]
        private Targeter targeter = null;

        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawned;

        public static event Action<Unit> AuthorityOnUnitSpawned;
        public static event Action<Unit> AuthorityOnUnitDespawned;

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
        }

        public override void OnStopServer()
        {
            base.OnStopClient();
            ServerOnUnitDespawned?.Invoke(this);
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

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!isClientOnly || !hasAuthority)
            {
                return;
            }
            AuthorityOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            if (!isClientOnly || !hasAuthority)
            {
                return;
            }
            AuthorityOnUnitDespawned?.Invoke(this);
        }
    }
}
