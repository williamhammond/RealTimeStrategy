﻿using System;
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
        private UnitMovement _unitMovement = null;

        public UnitMovement GetUnitMovement()
        {
            return _unitMovement;
        }

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

        #region Server



        #endregion
    }
}
