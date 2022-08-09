using System;
using Mirror;
using UnityEngine;

namespace Networking
{
    public class TeamColorSetter : NetworkBehaviour
    {
        [SerializeField]
        private Renderer[] colorRenderers = Array.Empty<Renderer>();

        [SyncVar(hook = nameof(HandleTeamColorUpdated))]
        private Color _teamColor;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        #region Server

        public override void OnStartServer()
        {
            var player = connectionToClient.identity.GetComponent<RTSPlayer>();

            _teamColor = player.GetTeamColor();
        }

        #endregion

        #region Client


        private void HandleTeamColorUpdated(Color oldColor, Color newColor)
        {
            foreach (var colorRenderer in colorRenderers)
            {
                colorRenderer.material.SetColor(BaseColor, newColor);
            }
        }

        #endregion
    }
}
