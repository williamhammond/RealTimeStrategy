using System;
using Mirror;
using Networking;
using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField]
    private Renderer[] colorRenderers = Array.Empty<Renderer>();

    [SyncVar(hook = nameof(HandleTeamColorUpdated))]
    private Color teamColor;

    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    #region Server

    public override void OnStartServer()
    {
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        teamColor = player.GetTeamColor();
    }

    #endregion

    #region Client


    private void HandleTeamColorUpdated(Color oldColor, Color newColor)
    {
        foreach (Renderer colorRenderer in colorRenderers)
        {
            colorRenderer.material.SetColor(BaseColor, newColor);
        }
    }

    #endregion
}
