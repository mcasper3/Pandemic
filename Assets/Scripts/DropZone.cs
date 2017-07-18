using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

    public enum DropZoneType { PLAYER, INFECTION, HAND, DISEASE, DISEASE_DISPOSAL };

    public DropZoneType dropZoneType = DropZoneType.HAND;
    public Text usedCardCounter;

    public List<GameObject> usedCards;
    private int usedCardCount;
    private GameObject usedCardDisplay;

    public void OnPointerExit(PointerEventData eventData)
    {
        var card = eventData.pointerDrag;

        if (card == null)
            return;

        Draggable draggable = card.GetComponent<Draggable>();

        if (draggable != null && draggable.placeholderParent != this.transform)
        {
            draggable.placeholderParent = draggable.parentToReturnTo;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var card = eventData.pointerDrag;

        if (card == null)
            return;

        Draggable draggable = card.GetComponent<Draggable>();

        if (draggable != null)
        {
            draggable.placeholderParent = this.transform;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var item = eventData.pointerDrag;
        Draggable draggable = item.GetComponent<Draggable>();
        CubeDraggable cubeDraggable = item.GetComponent<CubeDraggable>();

        if (draggable != null)
        {
            if (this.dropZoneType != DropZoneType.DISEASE)
            {
                draggable.parentToReturnTo = this.transform;

                if (dropZoneType == DropZoneType.HAND)
                {
                    draggable.newPosition = new Vector3(0, 0);
                }
                else if (draggable.itemType == this.dropZoneType)
                {
                    usedCards.Add(item);

                    if (usedCardCounter != null)
                        usedCardCounter.text = (++usedCardCount).ToString();

                    // Half the card's width (card is 56 x 83)
                    draggable.newPosition = new Vector3(28, 0);
                }
            }
        }
        else if (cubeDraggable != null)
        {
            if (this.dropZoneType == DropZoneType.DISEASE)
            {
                cubeDraggable.parentToReturnTo = this.transform;

                cubeDraggable.newPosition = eventData.position;
            }
            else if (this.dropZoneType == DropZoneType.DISEASE_DISPOSAL && !cubeDraggable.isStockCube)
            {
                this.transform.parent.GetComponent<GameInfo>().UpdateCubeCounter(cubeDraggable.cubeColor, false);
                Destroy(item);
            }
        }
    }

    public List<GameObject> GetUsedCards()
    {
        return usedCards;
    }

    public void ClearUsedCards()
    {
        usedCards.Clear();
    }

    public void OnClick()
    {
        if (usedCardDisplay == null)
        {
            if (this.transform.childCount > 2)
            {
                // TODO show all cards in the usedCards list
                usedCardDisplay = new GameObject();
                Image image = usedCardDisplay.AddComponent<Image>();
                image.color = new Color(73, 12, 94);

                RectTransform rect = usedCardDisplay.GetComponent<RectTransform>();
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 500);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 900);

                var gridLayout = usedCardDisplay.AddComponent<GridLayoutGroup>();
                gridLayout.cellSize = new Vector2(56, 83);
                gridLayout.childAlignment = TextAnchor.MiddleCenter;
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayout.constraintCount = 16;

                usedCardDisplay.transform.SetParent(this.transform.parent);
                usedCardDisplay.transform.position = this.transform.parent.position;

                foreach (var card in usedCards)
                {
                    card.transform.SetParent(usedCardDisplay.transform);
                    card.GetComponent<CanvasGroup>().blocksRaycasts = true;
                }

                usedCards.Clear();
            }
        }
        else
        {
            int numCards = usedCardDisplay.transform.childCount;

            for (int i = 0; i < numCards; i++)
            {
                GameObject card = usedCardDisplay.transform.GetChild(0).gameObject;
                card.transform.SetParent(this.transform);
                card.transform.SetSiblingIndex(card.transform.GetSiblingIndex() - 2);
                card.transform.localPosition = new Vector3(28, 0);
                card.GetComponent<CanvasGroup>().blocksRaycasts = false;
                usedCards.Add(card);
            }

            usedCardCount = usedCards.Count;
            usedCardCounter.text = usedCardCount.ToString();

            Destroy(usedCardDisplay);
            usedCardDisplay = null;
        }
    }

    private void Start()
    {
        usedCards = new List<GameObject>();
        usedCardCount = 0;
    }
}
