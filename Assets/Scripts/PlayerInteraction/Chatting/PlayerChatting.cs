using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class PlayerChatting : NetworkBehaviour
{
    InputField chatbarText;
    [SerializeField] GameObject chatBubble;
    [SerializeField] PlayerMovement playerMovement;
    MyPlayer myPlayer;
    ChatLogCanvas chatLogCanvas;

    public static event Action ClientOnReceivedMessage;

    #region Server
    
    [Command]
    public void CmdSendMessage(string message, Vector3 cellPosition)
    { 
        RpcHandleReceivedMessage(message, cellPosition);
    }

    #endregion


    #region Client 

    private void Update()
    {
        if (!Keyboard.current.enterKey.wasPressedThisFrame) { return; } // If player presses enter key

        if (chatbarText.text == "") { return; } // if player chatbar does not contain text return

        if (!isLocalPlayer) { return; } // if is not local player return

        CmdSendMessage(myPlayer.GetPlayerName() + ": " + chatbarText.text, playerMovement.currentCell.transform.position); // send message with player name & text with player cell position

        chatbarText.text = string.Empty; // Clear Chatbar Text
    }

    [Client]
    public override void OnStartClient()
    {
        base.OnStartClient();

        myPlayer = GetComponent<MyPlayer>();
        chatbarText = FindObjectOfType<Chatbar>().GetComponent<InputField>();
        chatLogCanvas = FindObjectOfType<ChatLogCanvas>();
    }

    [ClientRpc]
    public void RpcHandleReceivedMessage(string message, Vector3 cellPosition)
    {
        var screenPosition = Camera.main.WorldToScreenPoint(cellPosition);

        screenPosition.y = Screen.height * 2 / 3; // Sets screen position at 66.66% of screen height
        
        var newMessage = Instantiate(chatBubble, chatLogCanvas.transform); // instantiates chat bubble prefab on chatlog canvas

        newMessage.transform.position = screenPosition; // updates position of new bubble to screen position

        newMessage.GetComponentInChildren<Text>().text = message; // searches for text component in new bubble and adds message text to it

        ClientOnReceivedMessage?.Invoke(); // triggers client received message event
    }

    #endregion


    #region Helper

    #endregion


}
