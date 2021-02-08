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
        SpawnPlayer(playerInformation);
    }

    
    
    
    
    
    
    private void SpawnPlayer(WebSocketManager.PlayerInformation playerInformation)
    {
        if (playerInformation.GetPlayerNumber() == webSocketManager.myPlayerInformation.GetPlayerNumber()) // If its my player then spawn my player.
        {
            if (isMyPlayerSpawned == true) { return; }

            // spawn my player
            Player mySpawnedPlayer = Instantiate<Player>(playerPrefab, FindObjectOfType<Players>().transform);
            mySpawnedPlayer.PlayerName = playerInformation.PlayerName;
            mySpawnedPlayer.PlayerNumber = playerInformation.PlayerNumber;
            mySpawnedPlayer.Id = playerInformation.Id;
            mySpawnedPlayer.InRoom = playerInformation.InRoom;

            // Set player current cell to spawn.
            mySpawnedPlayer.playerMovement.currentCell = FindObjectOfType<Spawn>().GetComponent<Cell>(); // Set player spawn cell.
            mySpawnedPlayer.OnCell = FindObjectOfType<Spawn>().GetComponent<Cell>().cellNumber;

            // Send to server that I spawned with player information
            WebSocketManager.MessageContainer newMessageContainer = new WebSocketManager.MessageContainer(WebSocketManager.MessageType.Spawn, JsonUtility.ToJson(playerInformation));

            webSocketManager.ws.Send(Encoding.UTF8.GetBytes(JsonUtility.ToJson(newMessageContainer)));

            isMyPlayerSpawned = true;
        }
        else
        {
            // spawn the new player
            Player newSpawnedPlayer = Instantiate<Player>(playerPrefab, FindObjectOfType<Players>().transform);
            newSpawnedPlayer.PlayerName = playerInformation.PlayerName;
            newSpawnedPlayer.PlayerNumber = playerInformation.PlayerNumber;
            newSpawnedPlayer.Id = playerInformation.Id;
            newSpawnedPlayer.InRoom = playerInformation.InRoom;

            

            // Move and set player to proper cell
            Cell[] cellArray = FindObjectsOfType<Cell>();
            
            foreach(Cell cell in cellArray)
            {
                if (cell.cellNumber == playerInformation.OnCell)
                {
                    newSpawnedPlayer.playerMovement.currentCell = cell;
                    newSpawnedPlayer.OnCell = playerInformation.OnCell;
                }
            }

            
            
        }
    }


 


}
