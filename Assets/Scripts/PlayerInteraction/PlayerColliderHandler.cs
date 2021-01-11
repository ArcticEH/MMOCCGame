using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderHandler : MonoBehaviour
{
    [SerializeField] GameObject playerLabelCanvas;
    [SerializeField] LayerMask layerMask;


    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

        bool playerHit = false;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.gameObject.tag == "Player")
            {
                playerHit = true;
            }
        }

        playerLabelCanvas.SetActive(playerHit);
    }
}
