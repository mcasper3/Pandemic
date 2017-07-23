using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentToReturnTo;
    public Transform placeholderParent;
    public Transform originalParent;
    public Vector3 newPosition;
    public DropZone.DropZoneType itemType = DropZone.DropZoneType.PLAYER;

    GameObject placeholder;

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentToReturnTo = this.transform.parent;
        placeholderParent = parentToReturnTo;
        originalParent = parentToReturnTo;

        placeholder = new GameObject();
        var le = placeholder.AddComponent<LayoutElement>();
        le.preferredHeight = 83;
        le.preferredWidth = 56;
        placeholder.transform.SetParent(parentToReturnTo);
        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());

        this.transform.SetParent(parentToReturnTo.parent.parent);
        
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;

        int newSiblingIndex = placeholder.transform.GetSiblingIndex();

        if (placeholder.transform.parent != placeholderParent)
        {
            placeholder.transform.SetParent(placeholderParent);
        }

        for (int i = placeholderParent.childCount - 1; i >= 0; i--)
        {
            // Ew magic number for determining which row should be moveable
            if (this.transform.position.y - 41.5 > placeholderParent.GetChild(i).transform.position.y)
                continue;

            if (this.transform.position.x > placeholderParent.GetChild(i).position.x)
            {
                newSiblingIndex = i;

                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                    newSiblingIndex++;

                break;
            }
        }

        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(parentToReturnTo);

        if (newPosition.x > 0)
        {
            this.transform.localPosition = newPosition;
            this.transform.SetSiblingIndex(this.transform.GetSiblingIndex() - 3);

            // Being discarded
        }
        else
        {
            this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());

            // Being put in another person's hand
        }

        Destroy(placeholder);
    }
}
