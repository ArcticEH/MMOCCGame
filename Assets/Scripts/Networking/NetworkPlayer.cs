using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{

    public string PlayerName;
    public int PlayerNumber;
    public string Id;

    // Room information
    public string Room;

    public void SetSpawnedNetworkPlayerProperties(string playerName, int playerNumber, string id)
    {
        // Set self attributes
        PlayerName = playerName;
        PlayerNumber = playerNumber;
        Id = id;

        // Set player label
        GetComponentInChildren<PlayerLabel>().SetText($"Player {playerNumber}");

    }

    public void DespawnPlayer()
    {
        // Remove from the cell
        CellObject cellObject = GetComponent<CellObject>();
        cellObject.RemoveFromCell();

        // Destroy gameobject
        Destroy(this.gameObject);
    }

}
