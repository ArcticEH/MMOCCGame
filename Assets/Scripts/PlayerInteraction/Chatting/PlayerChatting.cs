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
    ChatLogCanvas chatLogCanvas;

    public static event Action ClientOnReceivedMessage;

    #region Server
    
    [Command]
    public void CmdSendMessage(string message)
    { 
        RpcHandleReceivedMessage(message);
    }

    #endregion


    #region Client

    private void Update()
    {
        if (!Keyboard.current.enterKey.wasPressedThisFrame) { return; }

        if (chatbarText.text == "") { return; }

        if (!isLocalPlayer) { return; }

        CmdSendMessage(chatbarText.text);

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
    public void RpcHandleReceivedMessage(string message)
    {
        var newMessage = Instantiate(chatBubble, chatLogCanvas.transform);

        newMessage.GetComponentInChildren<Text>().text = message;

        ClientOnReceivedMessage?.Invoke();
    }

    #endregion


    #region Helper

    #endregion


}
