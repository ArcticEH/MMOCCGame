using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

[Serializable]
public class Player : MonoBehaviour
{
    public WebSocketManager webSocketManager;
    public PlayerMovement playerMovement;

    [SerializeField] public string PlayerName;
    [SerializeField] public int PlayerNumber;
    [SerializeField] public string Id;
    [SerializeField] public string InRoom;
    [SerializeField] public int OnCell;

    // getters & setters

    public string GetPlayerName()
    {
        return PlayerName;
    }

    public int GetPlayerNumber()
    {
        return PlayerNumber;
    }

    public void SetPlayerName(string newName)
    {
        PlayerName = newName;
    }

    public void SetPlayerNumber(int newNumber)
    {
        PlayerNumber = newNumber;
    }

    public Player(string playerName, int playerNumber, string id, string inRoom, int onCell) // constructor
    {
        PlayerName = playerName;
        PlayerNumber = playerNumber;
        Id = id;
        InRoom = inRoom;
        OnCell = onCell;
    }

    private void Start()
    {
        webSocketManager = FindObjectOfType<WebSocketManager>();
    }
}
