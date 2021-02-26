using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    WebSocketManager webSocketManager;
    SceneManager sceneManager;

    // Start is called before the first frame update
    void Start()
    {
        webSocketManager = FindObjectOfType<WebSocketManager>();
    }

    public void Login()
    {
        // Get text from input field
        string playerName = FindObjectOfType<TMP_InputField>().text;
        FindObjectOfType<Button>().interactable = false;

        // Trigger login from web socket manager
        webSocketManager.playerName = playerName;
        webSocketManager.Connect();
    }

    public void NavigateToLobbyScene()
    {
        SceneManager.LoadScene(1);
    }

    public void EnterRoomOne()
    {
        webSocketManager.lastRoomIdJoined = 1;
        SceneManager.LoadScene(2);
    }


    public void EnterRoomTwo()
    {
        webSocketManager.lastRoomIdJoined = 2;
        SceneManager.LoadScene(3);
    }


}
