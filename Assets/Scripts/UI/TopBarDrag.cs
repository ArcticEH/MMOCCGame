using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TopBarDrag : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [SerializeField] private RectTransform dragRectTransform;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragRectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragRectTransform.anchoredPosition += eventData.delta;
    }
}
