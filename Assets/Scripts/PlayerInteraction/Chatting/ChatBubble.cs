using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
    [SerializeField] float timeBetweenScroll = 3f;

    private void Start()
    {
        PlayerChatting.OnReceivedChatMessage += Scroll; // subscribe to event which triggers when client receives a chat bubble message
    }


    private void Update()
    {
        timeBetweenScroll -= Time.deltaTime; // timer in realtime seconds

        if (timeBetweenScroll < 0)
        {
            Scroll();
            timeBetweenScroll = 3f;
        }
    }

    private void Scroll() // blinks chat bubble along the y axis by the bubble height
    {
        var rectTransform = GetComponent<RectTransform>();

        float nextPosition = rectTransform.anchoredPosition.y + rectTransform.rect.height;

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, nextPosition);

        timeBetweenScroll = 3f;
    }



}
