using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LoginCanvas : MonoBehaviour
{
    MyNetworkManager networkManager;

    private void Awake()
    {
        networkManager = FindObjectOfType<MyNetworkManager>();
    }

    public void Host()
    {
        networkManager.characterMessage.playerName = GetComponentInChildren<InputField>().text;
        networkManager.StartHost();
    }

    public void Client()
    {
        networkManager.characterMessage.playerName = GetComponentInChildren<InputField>().text;
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }
}
