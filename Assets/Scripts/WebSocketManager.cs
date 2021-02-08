using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Use plugin namespace
using HybridWebSocket;
using System;

public class WebSocketManager : MonoBehaviour
{
    [SerializeField] public PlayerInformation myPlayerInformation;
    public WebSocket ws;
    Queue<MessageContainer> receivedMessages = new Queue<MessageContainer>();

    [SerializeField] PlayerSpawner playerSpawner;

    private void Update()
    {
        if (receivedMessages.Count <= 0) { return; }

        MessageContainer nextMessage = receivedMessages.Dequeue(); Debug.Log("Message Deq'D");

        // Get My Player Information When I Connect to Server
        if(nextMessage.MessageType == MessageType.NewServerConnection)
        {
            // Get My Player Information When I Connect to Server
            PlayerInformation myGivenPlayerInformation = JsonUtility.FromJson<PlayerInformation>(nextMessage.Data);
            myPlayerInformation = myGivenPlayerInformation;

            playerSpawner.SpawnMessageHandler(myPlayerInformation); // temporary to spawn player here since game starts in public room. Change when player enters public rooms after.
        }
        else if (nextMessage.MessageType == MessageType.Spawn)
        {
            PlayerInformation playerInformation = JsonUtility.FromJson<PlayerInformation>(nextMessage.Data);
            playerSpawner.SpawnMessageHandler(playerInformation);
        }
        else if (nextMessage.MessageType == MessageType.Movement)
        {
            MovementData newMovementData = JsonUtility.FromJson<MovementData>(nextMessage.Data);

            Player[] playersArray = FindObjectOfType<Players>().GetComponentsInChildren<Player>();

            foreach(Player player in playersArray)
            {
                if (player.PlayerNumber == newMovementData.playerNumber)
                {
                    player.OnCell = newMovementData.destinationCellNumber;
                    player.playerMovement.HandleDestinationCellChanged(newMovementData.destinationCellNumber);
                    break;
                }
            }

        }
    }

    // Use this for initialization
    void Start()
    {
        // Create WebSocket instance
        ws = WebSocketFactory.CreateInstance("ws://localhost:9000/Chat");

        // Add OnOpen event listener
        ws.OnOpen += () =>
        {
            Debug.Log("Connected to Server!");
            Debug.Log("Server connection state: " + ws.GetState().ToString());
        };

        // Add OnMessage event listener
        ws.OnMessage += (byte[] msg) =>
        {
            MessageContainer receivedMessageContainer = JsonUtility.FromJson<MessageContainer>(Encoding.UTF8.GetString(msg));
            receivedMessages.Enqueue(receivedMessageContainer);
        };

        // Add OnError event listener
        ws.OnError += (string errMsg) => { Debug.Log("WS error: " + errMsg); };
            
        // Add OnClose event listener
        ws.OnClose += (WebSocketCloseCode code) => { Debug.Log("WS closed with code: " + code.ToString()); };

        // Connect to the server
        ws.Connect();
    }





    /// <summary>
    /// //////////////////////////////////// BEYOND HERE IS CLASS INFORMATION ////////////////////////////////////
    /// </summary>

    [Serializable]
    public class MessageContainer
    {
        [SerializeField] public MessageType MessageType;
        [SerializeField] public string Data;

        public MessageContainer(MessageType newMessageType, string newData)
        {
            MessageType = newMessageType;
            Data = newData;
        }
    }

    [Serializable]
    public class PlayerInformation
    {
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

        public PlayerInformation(string playerName, int playerNumber, string id, string inRoom, int onCell) // constructor
        {
            PlayerName = playerName;
            PlayerNumber = playerNumber;
            Id = id;
            InRoom = inRoom;
            OnCell = onCell;
        }
    }

    [Serializable]
    public abstract class MessageData { }

    [Serializable]
    public enum MessageType
    {
        NewServerConnection,
        Spawn,
        Movement,
        UpdateInformation
    }

    [Serializable]
    public class MovementData : MessageData
    {
        [SerializeField] public int playerNumber;
        [SerializeField] public int destinationCellNumber;

        public MovementData(int playerRequestingMovement, int destination)
        {
            playerNumber = playerRequestingMovement;
            destinationCellNumber = destination;
        }
    }

}
