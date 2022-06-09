using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject landingPagePanel = null;

    [SerializeField]
    private TMP_InputField addressInput = null;

    [SerializeField]
    private Button joinButton = null;

    public void OnEnable()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSNetworkManager.ClientOnConnected += HandleClientDisconnected;
    }

    public void OnDisable()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSNetworkManager.ClientOnConnected -= HandleClientDisconnected;
    }

    public void Join()
    {
        string address = addressInput.text;

        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        landingPagePanel.SetActive(false);
        gameObject.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
