using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayer : NetworkBehaviour
{
    [SerializeField] [SyncVar] string playerName;

    #region Server

    public override void OnStartServer()
    {
        // For now, create a name for the player
        int numPlayers = MyNetworkManager.Players.Count;
        playerName = $"Player {numPlayers}";
    }

   
    #endregion


    #region Client


    #endregion

    #region Helpers
    #endregion
}
