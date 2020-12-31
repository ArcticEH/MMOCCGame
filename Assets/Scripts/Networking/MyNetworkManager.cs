using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    // List of players
    [SerializeField] public List<MyPlayer> Players = new List<MyPlayer>();


    // Events
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;


    #region Server

    // Called when server adds a new player
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // Add to list of players
        MyPlayer player = conn.identity.GetComponent<MyPlayer>();
        Players.Add(player);
    }


    #endregion

    #region Client

    // Used when a client connects to server
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        ClientOnConnected?.Invoke();
    }

    // Used when a client disconnects from server
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ClientOnDisconnected?.Invoke();
    }

    #endregion
}
