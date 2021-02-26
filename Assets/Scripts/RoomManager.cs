using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] WebSocketManager webSocketManager;
    // Start is called before the first frame update
    void Start()
    {
        webSocketManager = FindObjectOfType<WebSocketManager>();

        // Spawn request for room
        SpawnRequest spawnRequest = new SpawnRequest()
        {
            playerId = webSocketManager.localNetworkPlayerId,
            roomId = webSocketManager.lastRoomIdJoined
        };

        string json = JsonUtility.ToJson(spawnRequest);
        webSocketManager.SendMessage(MessageType.SpawnRequest, json);
    }



}
