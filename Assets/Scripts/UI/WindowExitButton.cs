using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowExitButton : MonoBehaviour
{
    ExclamationIcon exclamationIcon;
    [SerializeField] GameObject parentWindow;

    private void Start()
    {
        exclamationIcon = FindObjectOfType<ExclamationIcon>();
    }

    public void CloseWindow()
    {
        exclamationIcon.SetIsWindowOpen(false);
        Destroy(parentWindow);
    }
}
