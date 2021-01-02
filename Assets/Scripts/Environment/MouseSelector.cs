using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelector : MonoBehaviour
{
    [SerializeField] public bool placingObject = false;
    [SerializeField] IsometricObject selectedObject;


    public IsometricObject GetSelectedObject()
    {
        return selectedObject;
    }
}
