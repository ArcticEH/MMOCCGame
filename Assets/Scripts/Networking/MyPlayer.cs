using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyPlayer : NetworkBehaviour
{
    [SerializeField] string playerName;

    #region Server

    public override void OnStartServer()
    {
        // Find spawn point in room and place in there
        var spawnPoint = FindObjectOfType<Spawn>();
        this.transform.parent = spawnPoint.transform;

    }

    #endregion

    #region Client


 

   #endregion

   #region Client


   #endregion
}
