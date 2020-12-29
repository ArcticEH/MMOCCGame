using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDimensions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer tileSprite = gameObject.GetComponent<SpriteRenderer>();

        Debug.Log(((float)tileSprite.bounds.extents.x).ToString());
       
    }

    
}
