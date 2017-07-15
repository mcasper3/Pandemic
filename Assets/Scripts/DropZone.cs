using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler {

    public enum DropZoneType { PLAYER, INFECTION, HAND };

    public DropZoneType dropZoneType = DropZoneType.HAND;
    public Text usedCardCounter;

    public List<GameObject> usedCards;
    private int usedCardCount;

    public void OnDrop(PointerEventData eventData)
    {
        var card = eventData.pointerDrag;
        Draggable draggable = card.GetComponent<Draggable>();

        if (draggable != null)
        {
            draggable.parentToReturnTo = this.transform;

            if (dropZoneType == DropZoneType.HAND)
            {
                draggable.newPosition = new Vector3(0, 0);
            }
            else if (draggable.cardType == this.dropZoneType)
            {
                usedCards.Add(card);
                usedCardCounter.text = (++usedCardCount).ToString();

                // Half the card's width and height (card is 56 x 83)
                draggable.newPosition = new Vector3(28, 0);
            }
        }
    }

    public void OnClick()
    {
        if (this.transform.childCount > 2)
        {
            // TODO show all cards in the usedCards list
        }
    }

    private void Start()
    {
        usedCards = new List<GameObject>();
        usedCardCount = 0;
    }
}
