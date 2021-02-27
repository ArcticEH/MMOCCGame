using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    // Two static references
    [SerializeField] WebSocketManager webSocketManager;

    private void Start()
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

    public void LeaveRoom()
    {
        // Send message to leave room
        DespawnRequest despawnRequest = new DespawnRequest
        {
            Id = webSocketManager.localNetworkPlayerId,
            RoomId = webSocketManager.lastRoomIdJoined
        };

        webSocketManager.SendMessage(MessageType.DespawnRequest, JsonUtility.ToJson(despawnRequest));

        // Leave room
        webSocketManager.lastRoomIdJoined = -1;
        SceneManager.LoadScene(1);
    }


}
