using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentToReturnTo;
    public Vector3 newPosition;
    public DropZone.DropZoneType cardType = DropZone.DropZoneType.PLAYER;

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(parentToReturnTo.parent.parent);

        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(parentToReturnTo);

        if (newPosition.x > 0)
        {
            this.transform.localPosition = newPosition;
            this.transform.SetSiblingIndex(this.transform.GetSiblingIndex() - 2);
        }
        else
        {
            this.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
}
