using Buildings;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class Targeter : NetworkBehaviour
    {
        private Targetable target;

        public Targetable GetTarget()
        {
            return target;
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
            if (!targetGameObject.TryGetComponent(out Targetable newTarget))
            {
                return;
            }
            target = newTarget;
        }

        [Server]
        public void ClearTarget()
        {
            target = null;
        }
        #endregion
    }
}
