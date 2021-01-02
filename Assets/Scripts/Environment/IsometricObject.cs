using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class IsometricObject : MonoBehaviour
{
    //[SerializeField] private float height = 0;
    [SerializeField] private bool doesItBlockPath;

    #region Server

    // Determines if player should be able to walk on this cell. Future implementation may consider more factors than just the variable.
    public bool DoesObjectBlockPath() {
        return doesItBlockPath;
    }

    #endregion

    #region Client
    #endregion

}
