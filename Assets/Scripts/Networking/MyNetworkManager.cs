using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyNetworkManager : NetworkManager
{
    // List of players
    [SerializeField] static public List<MyPlayer> Players = new List<MyPlayer>();

    public CreateCharacterMessage characterMessage;

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

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        // Remove from list of players
        MyPlayer player = conn.identity.GetComponent<MyPlayer>();
        Players.Remove(player);
    }

    public void OnSpawnPlayer(NetworkConnection conn, CreateCharacterMessage message)
    {
        var newPlayer = Instantiate(playerPrefab);

        NetworkServer.AddPlayerForConnection(conn, newPlayer);
    }
    #endregion




    #region Client

    public override void Awake()
    {
        base.Awake();
    }

    // Used when a client connects to server
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        conn.Send(characterMessage);

        ClientOnConnected?.Invoke();
    }

    // Used when a client disconnects from server
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ClientOnDisconnected?.Invoke();
    }

    //Spawn the Player
    public void SpawnPlayer()
    {
        NetworkServer.RegisterHandler<CreateCharacterMessage>(OnSpawnPlayer);
    }

    #endregion
}
