using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
    [SerializeField] float timeBetweenScroll = 3f;

    private void Start()
    {
        PlayerChatting.ClientOnReceivedMessage += Scroll;
    }


    private void Update()
    {
        timeBetweenScroll -= Time.deltaTime;

        if (timeBetweenScroll < 0)
        {
            Scroll();
            timeBetweenScroll = 3f;
        }
    }



    //IEnumerator ScrollOffTime()
    //{
    //    var rectTransform = GetComponent<RectTransform>();

    //    float nextPosition = rectTransform.anchoredPosition.y + rectTransform.rect.height;

    //    while (rectTransform.anchoredPosition.y < nextPosition)
    //    {
    //        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + 0.5f);

    //        yield return null;
    //    }

    //    yield break;
    //}

    private void Scroll()
    {
        var rectTransform = GetComponent<RectTransform>();

        float nextPosition = rectTransform.anchoredPosition.y + rectTransform.rect.height;

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, nextPosition);

        timeBetweenScroll = 3f;
    }



}
