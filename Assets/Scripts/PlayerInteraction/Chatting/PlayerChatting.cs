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
        if (!Keyboard.current.enterKey.wasPressedThisFrame) { return; }

        if (chatbarText.text == "") { return; }

        if (!isLocalPlayer) { return; }

        CmdSendMessage(chatbarText.text, playerMovement.currentCell.transform.position);

        chatbarText.text = string.Empty;
    }

    [Client]
    public override void OnStartClient()
    {
        base.OnStartClient();

        chatbarText = FindObjectOfType<Chatbar>().GetComponent<InputField>();
        chatLogCanvas = FindObjectOfType<ChatLogCanvas>();
    }

    [ClientRpc]
    public void RpcHandleReceivedMessage(string message, Vector3 cellPosition)
    {

        var screenPosition = Camera.main.WorldToScreenPoint(cellPosition);

        screenPosition.y = Screen.height * 2 / 3;
        
        var newMessage = Instantiate(chatBubble, chatLogCanvas.transform);

        newMessage.transform.position = screenPosition;

        newMessage.GetComponentInChildren<Text>().text = message;

        ClientOnReceivedMessage?.Invoke();
    }

    #endregion


    #region Helper

    #endregion


}
