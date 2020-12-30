using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class IsometricObject : NetworkBehaviour
{
    [SerializeField] private float height = 0;
    [SerializeField] private bool isWalkable;

    #region Server

    // Determines if player should be able to walk on this cell. Future implementation may consider more factors than just the variable.
    public bool IsCellWalkable() {
        return isWalkable;
    }

    #endregion

    #region Client
    #endregion

}
