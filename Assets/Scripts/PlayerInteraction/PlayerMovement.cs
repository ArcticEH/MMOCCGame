using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    #region Server

    [Command]
    public void MoveToCell()
    {
        // Check if the cell is allowed to be move to


    }

    #endregion


    #region Client


    #endregion
}
