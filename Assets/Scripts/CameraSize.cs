using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSize : MonoBehaviour
{
    void Start()
    {
        float newCameraSize = (float)Screen.height / 2 / 1;
        Camera.main.orthographicSize = newCameraSize;
    }
}
