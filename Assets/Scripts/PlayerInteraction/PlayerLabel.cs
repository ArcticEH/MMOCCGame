using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLabel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerLabelText;

    public void SetText(string text)
    {
        playerLabelText.SetText(text);
    }

}
