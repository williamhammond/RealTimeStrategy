using Buildings;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class Targeter : NetworkBehaviour
    {
        private Targetable _target;

        public Targetable GetTarget()
        {
            return _target;
        }

        #region Server
        public override void OnStartServer()
        {
            GameoverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            GameoverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        [Server]
        private void ServerHandleGameOver()
        {
            ClearTarget();
        }

        [Command]
        public void CmdSetTarget(GameObject targetGameObject)
        {
            if (!targetGameObject.TryGetComponent<Targetable>(out Targetable target))
            {
                return;
            }
            this._target = target;
        }

        [Server]
        public void ClearTarget()
        {
            _target = null;
        }
        #endregion
    }
}
