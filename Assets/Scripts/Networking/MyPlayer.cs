using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayer : NetworkBehaviour
{
    [SerializeField] [SyncVar] string playerName;

    [SerializeField] TMP_Text playerLabelText; 

    public string GetPlayerName()
    {
        return playerName;
    }

    #region Server

    public override void OnStartServer()
    {
        // For now, create a name for the player
        int numPlayers = MyNetworkManager.Players.Count;
        playerName = $"Player {numPlayers}";
    }


    #endregion


    #region Client

    private void Start()
    {
        playerLabelText.text = playerName;
    }

    #endregion

    #region Helpers
    #endregion
}
