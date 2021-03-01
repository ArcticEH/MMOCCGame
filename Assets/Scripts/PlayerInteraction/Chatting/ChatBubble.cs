using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
    [SerializeField] float timeBetweenScroll = 3f;
    RectTransform rectTransform;
    ChatLogCanvas chatlogCanvas;

    private void Start()
    {
        FindObjectOfType<PlayerChatting>().OnReceivedChatMessage += Scroll; // subscribe to event which triggers when client receives a chat bubble message
        rectTransform = GetComponent<RectTransform>();
        chatlogCanvas = FindObjectOfType<ChatLogCanvas>();
    }


    private void Update()
    {
        timeBetweenScroll -= Time.deltaTime; // timer in realtime seconds

        if (timeBetweenScroll < 0)
        {
            Scroll();
            timeBetweenScroll = 3f;
        }

        CheckToDestroy(rectTransform);
    }


    private void Scroll() // blinks chat bubble along the y axis by the bubble height
    {
        float nextPosition = rectTransform.anchoredPosition.y + rectTransform.rect.height;

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, nextPosition);

        timeBetweenScroll = 3f;
    }

    private void CheckToDestroy(RectTransform rectTransform)
    {
        if (rectTransform.anchoredPosition.y > chatlogCanvas.GetComponent<RectTransform>().anchoredPosition.y) 
        {
            FindObjectOfType<PlayerChatting>().OnReceivedChatMessage -= Scroll;  //unsubscribe to event which triggers when client receives a chat bubble message
            Destroy(this.gameObject);
        }
    }



}
