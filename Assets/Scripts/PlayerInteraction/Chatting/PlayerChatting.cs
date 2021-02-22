﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class PlayerChatting : MonoBehaviour
{
    // Player Chatting
    [SerializeField] ChatBubble chatBubblePrefab;
    

    // Events
    public static Action OnReceivedChatMessage;

    // Canvases
    Chatbar chatBar;
    ChatLogCanvas chatLogCanvas;
    WindowCanvas windowsCanvas;

    // Caches
    WebSocketManager webSocketManager;
    
    
    private void Awake()
    {
        webSocketManager = FindObjectOfType<WebSocketManager>();
        chatBar = FindObjectOfType<Chatbar>();
        chatLogCanvas = FindObjectOfType<ChatLogCanvas>();
        chatBar.GetComponent<InputField>().ActivateInputField();
    }

    private void Update()
    {
        // temporary until I figure out how to check if input field is activated to then activate input field. I want input field always active.
        chatBar.GetComponent<InputField>().ActivateInputField();
        
        if (!Keyboard.current.enterKey.wasPressedThisFrame) { return; }

        var playerList = FindObjectsOfType<NetworkPlayer>();

        string myPlayerID = webSocketManager.localNetworkPlayerId;

        string myPlayerName = "not found";
        float myPlayerXLocation = default;

        foreach (NetworkPlayer player in playerList)
        {
            if (player.Id == myPlayerID)
            {
                myPlayerName = player.PlayerName;
                myPlayerXLocation = player.transform.position.x;
            }
        }

        string chatMessage = myPlayerName + ": " + chatBar.GetComponent<InputField>().text;

        InRoomChatMessageData newChatMessageData = new InRoomChatMessageData();
        newChatMessageData.chatMessage = chatMessage;
        newChatMessageData.messageXLocation = myPlayerXLocation;
        newChatMessageData.roomName = "Welcome";

        webSocketManager.SendMessage(MessageType.InRoomChatMessage, JsonUtility.ToJson(newChatMessageData));

        chatBar.GetComponent<InputField>().text = "";
    }

    public void HandleReceivedInRoomMessage(InRoomChatMessageData messageData)
    {
        OnReceivedChatMessage?.Invoke();

        var newChatBubble = Instantiate(chatBubblePrefab, FindObjectOfType<ChatLogCanvas>().transform);

        newChatBubble.GetComponentInChildren<Text>().text = messageData.chatMessage;
        newChatBubble.transform.position = FindObjectOfType<Camera>().WorldToScreenPoint(new Vector3(messageData.messageXLocation, Screen.height * 0.20f, 0f));
    }

}
