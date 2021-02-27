using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using TMPro;

public class PlayerChatting : MonoBehaviour
{
    // Player Chatting
    [SerializeField] ChatBubble chatBubblePrefab;
    

    // Events
    public static Action OnReceivedChatMessage;

    // Canvases
    [SerializeField] Chatbar chatBar;
    ChatLogCanvas chatLogCanvas;
    WindowCanvas windowsCanvas;

    // Caches
    WebSocketManager webSocketManager;
    
    
    private void Awake()
    {
        webSocketManager = FindObjectOfType<WebSocketManager>();
        chatBar = FindObjectOfType<Chatbar>();
        chatLogCanvas = FindObjectOfType<ChatLogCanvas>();
        chatBar.GetComponent<TMP_InputField>().ActivateInputField();
        windowsCanvas = FindObjectOfType<WindowCanvas>();
    }

    private void Update()
    {
        // temporary until I figure out how to check if input field is activated to then activate input field. I want input field always active.
        chatBar.GetComponent<TMP_InputField>().ActivateInputField();
        
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

        string chatMessage = myPlayerName + ": " + chatBar.GetComponent<TMP_InputField>().text;

        InRoomChatMessageData newChatMessageData = new InRoomChatMessageData();

        // TODO: create this properly
        newChatMessageData.chatMessage = chatMessage;
        newChatMessageData.messageXLocation = myPlayerXLocation;
        newChatMessageData.roomId = webSocketManager.lastRoomIdJoined;

        webSocketManager.SendMessage(MessageType.InRoomChatMessage, JsonUtility.ToJson(newChatMessageData));

        chatBar.GetComponent<TMP_InputField>().text = "";
    }

    public IEnumerator HandleReceivedInRoomMessage(InRoomChatMessageData messageData)
    {
        OnReceivedChatMessage?.Invoke();

        var newChatBubble = Instantiate(chatBubblePrefab, FindObjectOfType<ChatLogCanvas>().transform);
        Camera camera = FindObjectOfType<Camera>();

        newChatBubble.GetComponentInChildren<Text>().text = messageData.chatMessage;
        newChatBubble.transform.position = camera.WorldToScreenPoint(new Vector3(messageData.messageXLocation, Screen.height * 0.20f, 0f));

        yield return new WaitForSeconds(0.2f);
        RectTransform newBubbleRectTransform = newChatBubble.GetComponent<RectTransform>();
        CheckIfInBounds(newBubbleRectTransform.anchoredPosition.x, newBubbleRectTransform.rect.width, newChatBubble);
    }


    public void CheckIfInBounds(float xPosition, float bubbleWidth, ChatBubble newChatBubble)
    {
        float rightSideOfScreen = chatLogCanvas.GetComponent<RectTransform>().rect.width / 2;
        float leftSideOfScreen = (chatLogCanvas.GetComponent<RectTransform>().rect.width / 2) * -1;

        if (xPosition - bubbleWidth / 2 < leftSideOfScreen)
        {
            Debug.Log("OFF LEFT SIDE OF SCREEN");
            RectTransform rt = newChatBubble.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(leftSideOfScreen + bubbleWidth / 2, rt.anchoredPosition.y);
        }
        
        if (xPosition + bubbleWidth / 2 > rightSideOfScreen)
        {
            Debug.Log("OFF RIGHT SIDE OF SCREEN");
            RectTransform rt = newChatBubble.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rightSideOfScreen - bubbleWidth / 2, rt.anchoredPosition.y);
        }
    }

}
