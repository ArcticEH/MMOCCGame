using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExclamationIcon : MonoBehaviour
{
    WindowCanvas windowCanvas;
    [SerializeField] ExclamationWindow exclamationWindow;

    private bool isWindowOpen = false;

    public void SetIsWindowOpen(bool nextWindowState)
    {
        isWindowOpen = nextWindowState;
    }

    private void Start()
    {
        windowCanvas = FindObjectOfType<WindowCanvas>();
    }

    public void OpenWindow()
    {
        if (isWindowOpen == false)
        {
            Instantiate(exclamationWindow, windowCanvas.transform);
            isWindowOpen = true;
        } 
        else
        {
            var activeExclamationWindow = FindObjectOfType<ExclamationWindow>();
            activeExclamationWindow.DestroyWindow();
            isWindowOpen = false;
        }
    }

    public void HighlightIcon()
    {
        var image = GetComponent<Image>().color = Color.grey;
    }

    public void UnHighlightIcon()
    {
        var image = GetComponent<Image>().color = Color.white;
    }
}
