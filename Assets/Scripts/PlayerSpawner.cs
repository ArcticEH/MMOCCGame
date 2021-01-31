using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HybridWebSocket;
using System.Text;
using System;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] WebSocketManager webSocketManager;
    string publicRoomName = "default";
    [SerializeField] Player playerPrefab;

    bool isMyPlayerSpawned = false;

    private void Start()
    {
        webSocketManager = FindObjectOfType<WebSocketManager>();
    }

    public void SpawnMessageHandler(WebSocketManager.PlayerInformation playerInformation)
    {
        if (playerInformation.GetPlayerNumber() == FindObjectOfType<WebSocketManager>().myPlayerInformation.PlayerNumber) // If its my player then spawn my player.
        {
            if (isMyPlayerSpawned == true) { return; }

            // spawn my player
            Player mySpawnedPlayer = Instantiate<Player>(playerPrefab, FindObjectOfType<Players>().transform);
            mySpawnedPlayer.PlayerName = playerInformation.PlayerName;
            mySpawnedPlayer.PlayerNumber = playerInformation.PlayerNumber;
            mySpawnedPlayer.Id = playerInformation.Id;
            mySpawnedPlayer.InRoom = playerInformation.InRoom;
            mySpawnedPlayer.OnCell = playerInformation.OnCell;

            // Move player to spawn cell.
            mySpawnedPlayer.transform.position = FindObjectOfType<Spawn>().transform.position;

            // Send to server that I spawned with player information
            WebSocketManager.MessageContainer newMessageContainer = new WebSocketManager.MessageContainer(WebSocketManager.MessageType.Spawn, JsonUtility.ToJson(playerInformation));

            webSocketManager.ws.Send(Encoding.UTF8.GetBytes(JsonUtility.ToJson(newMessageContainer)));

            isMyPlayerSpawned = true;
        }
        else if (playerInformation.GetPlayerNumber() != FindObjectOfType<WebSocketManager>().myPlayerInformation.PlayerNumber)
        {
            // spawn the new player
            Player newSpawnedPlayer = Instantiate<Player>(playerPrefab, FindObjectOfType<Players>().transform);

            Debug.Log(newSpawnedPlayer);
            newSpawnedPlayer.PlayerName = playerInformation.PlayerName;
            newSpawnedPlayer.PlayerNumber = playerInformation.PlayerNumber;
            newSpawnedPlayer.Id = playerInformation.Id;
            newSpawnedPlayer.InRoom = playerInformation.InRoom;
            newSpawnedPlayer.OnCell = playerInformation.OnCell;

            // Move player to spawn cell
            newSpawnedPlayer.transform.position = FindObjectOfType<Spawn>().transform.position;
        }
    }


 


}
