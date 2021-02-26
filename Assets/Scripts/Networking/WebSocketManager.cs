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
    private static WebSocketManager instance;

    [SerializeField] public string localNetworkPlayerId;

    public NetworkPlayer loggedInNetworkPlayer;

    [SerializeField] NetworkPlayer playerPrefab;

    [SerializeField] public String playerName;

    [SerializeField] public GameSceneManager gameSceneManager;

    public WebSocket ws;

    [SerializeField] public int lastRoomIdJoined = -1;

    // Queue used to receive messages from websocket events
    Queue<MessageContainer>  receivedMessages = new Queue<MessageContainer>();

    private void Awake()
    {
        // Singleton approach 
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

    }

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

    }

    public void Connect()
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
        Debug.Log(messageContainer.MessageData);

        switch (messageContainer.MessageType)
        {
            case MessageType.NewServerConnection:
                // Add as local player
                Debug.Log("Got new server connection");
                
                NewServerConnectionData newServerConnectionData = JsonUtility.FromJson<NewServerConnectionData>(messageContainer.MessageData);

                // Put in local network player temporarily
                localNetworkPlayerId = newServerConnectionData.Id;

                // Send login request with information
                Login login = new Login()
                {
                    PlayerName = playerName,
                    playerId = localNetworkPlayerId
                };

                SendMessage(MessageType.Login, JsonUtility.ToJson(login));
                break;

            case MessageType.LoginResponse:
                Debug.Log("Got player login");
                LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(messageContainer.MessageData);

                // Return if error from server (dont login)
                if (!loginResponse.isSuccess)
                {
                    Debug.Log("Error message received from server on login");
                    return;
                }

                // Set logged in networkPlayer info
                loggedInNetworkPlayer = new NetworkPlayer()
                {
                    Id = localNetworkPlayerId,
                    PlayerName = playerName,
                };

                // Open lobby scene
                gameSceneManager.NavigateToLobbyScene();

                break;
            case MessageType.SpawnResponse:
                Debug.Log("Got message to spawn EXISTING player");
                // Spawn existing player in room
                SpawnResponse existingSpawnData = JsonUtility.FromJson<SpawnResponse>(messageContainer.MessageData);
                var existingPlayer = Instantiate(playerPrefab);
                NetworkPlayer existingPlayerNetworkPlayer = existingPlayer.GetComponent<NetworkPlayer>();
                existingPlayerNetworkPlayer.SetSpawnedNetworkPlayerProperties("", existingSpawnData.playerNumber, existingSpawnData.playerId);
                existingPlayer.GetComponent<PlayerMovement>().SpawnPlayer(existingSpawnData);
                print($"Giving player {existingSpawnData.playerId} cell: {existingSpawnData.cellNumber}");
                break;
            case MessageType.DespawnData:
                Debug.Log("Got message to despawn");
                // Despawn player
                DespawnData despawnData = JsonUtility.FromJson<DespawnData>(messageContainer.MessageData);
                var playerToDespawn = FindObjectsOfType<NetworkPlayer>().Where(networkPlayer => networkPlayer.Id.Equals(despawnData.Id)).FirstOrDefault();
                playerToDespawn.DespawnPlayer();
                break;
            case MessageType.MovementDataUpdate:
                //Debug.Log("Got player movement");
                // Move player
                MovementDataUpdate movementDataUpdate = JsonUtility.FromJson<MovementDataUpdate>(messageContainer.MessageData);
                // Find player with the id
                NetworkPlayer[] networkPlayers = FindObjectsOfType<NetworkPlayer>();
                NetworkPlayer playerToMove = networkPlayers.Where(networkPlayer => networkPlayer.Id.Equals(movementDataUpdate.playerId)).FirstOrDefault();
                playerToMove.GetComponent<PlayerMovement>().HandlePlayerPositionChanged(movementDataUpdate);
                break;
            case MessageType.InRoomChatMessage:
                InRoomChatMessageData messageData = JsonUtility.FromJson<InRoomChatMessageData>(messageContainer.MessageData);
                PlayerChatting playerChatting = FindObjectOfType<PlayerChatting>();
                playerChatting.HandleReceivedInRoomMessage(messageData);
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
