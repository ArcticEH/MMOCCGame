using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyPlayer : NetworkBehaviour
{

    [SerializeField] string playerName;
    [SyncVar] public int playerCell;

    #region Server

    [Server]
    public override void OnStartClient()
    {
        base.OnStartClient();
        RpcHandleClientEnterRoom(); // Currently happens right on spawn but should happen only when entering a new room in the future
        TargetClientSpawnOthers(connectionToClient);

        print("Connection to client:");
        Debug.Log(connectionToClient);
    }

    #endregion


    #region Client

    [ClientRpc]
    private void RpcHandleClientEnterRoom()
    {
        MyNetworkManager nm = FindObjectOfType<MyNetworkManager>();
        Debug.LogError("Called on Client");
        // Find spawn point in room and place in there
        var spawnPoint = FindObjectOfType<Spawn>();
        transform.parent = spawnPoint.transform;
        GetComponent<PlayerMovement>().currentCell = spawnPoint.GetComponent<Cell>();
    }

    [TargetRpc]
    private void TargetClientSpawnOthers(NetworkConnection connectionToClient)
    {

        Debug.LogError("changing positions of all others");


        foreach (MyPlayer player in MyNetworkManager.Players)
        {
            Cell playerCell = player.GetComponent<PlayerMovement>().currentCell;
            player.transform.position = new Vector2(0, 0);
            player.transform.parent = playerCell.transform;
        }
    }

    #endregion
}
