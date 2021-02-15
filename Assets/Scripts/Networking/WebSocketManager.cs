using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Newtonsoft.Json; // Added using https://docs.microsoft.com/en-us/visualstudio/gamedev/unity/unity-scripting-upgrade

// Use plugin namespace
using HybridWebSocket;
using System;
using System.IO;
using System.Text;
using System.Linq;

public class WebSocketManager : MonoBehaviour
{
    [SerializeField] public string localNetworkPlayerId;

    [SerializeField] NetworkPlayer playerPrefab;

    public WebSocket ws;

    // Queue used to receive messages from websocket events
    Queue<MessageContainer>  receivedMessages = new Queue<MessageContainer>();

    private void Update()
    {
        // Handle received messages
        while(receivedMessages.Count != 0)
        {
            // Send to message handler
            HandleMessage(receivedMessages.Dequeue());
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

            // Handle deserialization process
            string json = Encoding.UTF8.GetString(msg); 
            MessageContainer messageContainer = JsonUtility.FromJson<MessageContainer>(json);

            // Add it to the queue
            receivedMessages.Enqueue(messageContainer);
        };

        // Add OnError event listener
        ws.OnError += (string errMsg) => { Debug.Log("WS error: " + errMsg); };
            
        // Add OnClose event listener
        ws.OnClose += (WebSocketCloseCode code) => { Debug.Log("WS closed with code: " + code.ToString()); };

        // Connect to the server
        ws.Connect();
    }

    private void HandleMessage(MessageContainer messageContainer)
    {
        switch (messageContainer.MessageType)
        {
            case MessageType.NewServerConnection:
                // Add as local player

                Debug.Log("Got new server connection");
                //NewServerConnectionData newServerConnectionData = JsonConvert.DeserializeObject<NewServerConnectionData>(messageContainer.MessageData);
                NewServerConnectionData newServerConnectionData = JsonUtility.FromJson<NewServerConnectionData>(messageContainer.MessageData);

                // Put in local network player temporarily
                localNetworkPlayerId = newServerConnectionData.Id;

                // For now we immediately request to spawn
                SpawnData spawnData = new SpawnData
                {
                    playerId = localNetworkPlayerId,
                    playerNumber = newServerConnectionData.PlayerNumber
                };

                SendMessage(MessageType.NewSpawn, JsonUtility.ToJson(spawnData));
                break;
            case MessageType.ExistingSpawn:
                Debug.Log("Got message to spawn EXISTING player");
                // Spawn existing player in room
                ExistingSpawnData existingSpawnData = JsonUtility.FromJson<ExistingSpawnData>(messageContainer.MessageData);
                var existingPlayer = Instantiate(playerPrefab);
                NetworkPlayer existingPlayerNetworkPlayer = existingPlayer.GetComponent<NetworkPlayer>();
                existingPlayerNetworkPlayer.Id = existingSpawnData.Id;
                existingPlayerNetworkPlayer.PlayerNumber = existingSpawnData.playerNumber;
                existingPlayerNetworkPlayer.GetComponentInChildren<PlayerLabel>().SetText($"Player {existingSpawnData.playerNumber}");
                existingPlayer.GetComponent<PlayerMovement>().currentCell = PlayerMovement.FindCellWithNumberTwo(existingSpawnData.cellNumber);
                break;
            case MessageType.NewSpawn:
                Debug.Log("Got message to spawn NEW player");
                // Spawn player
                SpawnData returnedSpawnData = JsonUtility.FromJson<SpawnData>(messageContainer.MessageData);
                var player = Instantiate(playerPrefab);
                var newSpawnNetworkPlayer = player.GetComponent<NetworkPlayer>();
                newSpawnNetworkPlayer.Id = returnedSpawnData.playerId;
                newSpawnNetworkPlayer.PlayerNumber = returnedSpawnData.playerNumber;
                player.GetComponentInChildren<PlayerLabel>().SetText($"Player {newSpawnNetworkPlayer.PlayerNumber}");
                player.GetComponent<PlayerMovement>().currentCell = FindObjectOfType<Spawn>().GetComponent<Cell>();
                break;
            case MessageType.Movement:
                Debug.Log("Got player movement");
                // Move player
                MovementData movementData = JsonUtility.FromJson<MovementData>(messageContainer.MessageData);
                // Find player with the id
                NetworkPlayer[] networkPlayers = FindObjectsOfType<NetworkPlayer>();
                NetworkPlayer playerToMove = networkPlayers.Where(networkPlayer => networkPlayer.Id.Equals(movementData.playerId)).FirstOrDefault();
                playerToMove.GetComponent<PlayerMovement>().HandleDestinationCellChanged(movementData.destinationCellNumber);
                break;
        }
    }

    public void SendMessage(MessageType messageType, string jsonMessage)
    {
        // Convert into message container
        MessageContainer messageContainer = new MessageContainer(messageType, jsonMessage);

        // Encode and send message
        //string json = JsonConvert.SerializeObject(messageContainer);
        string json = JsonUtility.ToJson(messageContainer);
        ws.Send(Encoding.UTF8.GetBytes(json));          
    }

}
