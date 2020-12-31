using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyPlayer : NetworkBehaviour
{

    [SerializeField] string playerName;

    #region Server

    [Server]
    public override void OnStartClient()
    {
        base.OnStartClient();
        HandleClientEnterRoom(); // Currently happens right on spawn but should happen only when entering a new room in the future
    }

    #endregion


    #region Client

    [ClientRpc]
    private void HandleClientEnterRoom()
    {
        Debug.LogError("Called on Client");
        // Find spawn point in room and place in there
        var spawnPoint = FindObjectOfType<Spawn>();
        this.transform.parent = spawnPoint.transform;
    }

    #endregion
}
