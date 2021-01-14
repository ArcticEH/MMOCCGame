using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : NetworkBehaviour
{
    MyNetworkManager networkManager;

    private void Awake()
    {
        networkManager = FindObjectOfType<MyNetworkManager>();
        networkManager.SpawnPlayer();
    }
}
