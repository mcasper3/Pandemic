using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
    public const int SYNC_PLAYER_DISCARD = 120;
    public const int SYNC_INFECTION_DISCARD = 121;

    public enum DropZoneType { PLAYER, INFECTION, HAND, DISEASE, DISEASE_DISPOSAL };

    public DropZoneType dropZoneType = DropZoneType.HAND;
    public Text usedCardCounter;
    public GameObject cardPrefab;

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
                if (dropZoneType == DropZoneType.HAND)
                {
                    draggable.parentToReturnTo = this.transform;
                    draggable.newPosition = new Vector3(0, 0);

                    var playerHand = draggable.originalParent.GetComponent<PlayerHand>();

                    if (playerHand != null)
                        playerHand.RemoveCard(draggable.gameObject);

                    this.GetComponent<PlayerHand>().AddCard(draggable.gameObject);
                }
                else if (draggable.itemType == this.dropZoneType)
                {
                    usedCards.Add(item);
                    draggable.parentToReturnTo = this.transform;

                    if (usedCardCounter != null)
                        usedCardCounter.text = (++usedCardCount).ToString();

                    // Half the card's width (card is 56 x 83)
                    draggable.newPosition = new Vector3(28, 0);

                    draggable.originalParent.GetComponent<PlayerHand>().RemoveCard(draggable.gameObject);
                }

                GameObject.FindObjectOfType<Canvas>().GetComponent<GameManager>().SyncAllHandsAndDiscard();
                SyncDiscard();
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
                float[] destroyPosition = new float[] { cubeDraggable.originalPosition.x, cubeDraggable.originalPosition.y, cubeDraggable.originalPosition.z };

                PhotonNetwork.RaiseEvent(GameManager.DESTROY_CUBE, destroyPosition, true, null);

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
        foreach (var card in usedCards)
        {
            Destroy(card);
        }
        usedCards.Clear();
    }

    public void OnClick()
    {
        if (usedCardDisplay == null)
        {
            if (this.transform.childCount > 2)
            {
                usedCardDisplay = new GameObject();
                Image image = usedCardDisplay.AddComponent<Image>();
                image.color = new Color32(73, 12, 94, 255);

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

            usedCards.Clear();

            for (int i = 0; i < numCards; i++)
            {
                GameObject card = usedCardDisplay.transform.GetChild(0).gameObject;
                card.transform.SetParent(this.transform);
                card.transform.SetSiblingIndex(this.transform.childCount - 3);
                card.transform.localPosition = new Vector3(28, 0);
                card.GetComponent<CanvasGroup>().blocksRaycasts = false;
                usedCards.Add(card);
            }

            usedCardCount = usedCards.Count;
            usedCardCounter.text = usedCardCount.ToString();

            Destroy(usedCardDisplay);
            usedCardDisplay = null;

            GameObject.FindObjectOfType<Canvas>().GetComponent<GameManager>().SyncAllHandsAndDiscard();

            SyncDiscard();
        }
    }

    public void SyncDiscard()
    {
        usedCardCount = usedCards.Count;
        Debug.Log("Used card count: " + usedCardCount);

        usedCardCounter.text = usedCardCount.ToString();

        int[] info = new int[usedCardCount];
        for (int i = 0; i < usedCardCount; i++)
        {
            info[i] = usedCards[i].GetComponent<CardModel>().cardPosition;
        }

        if (this.dropZoneType == DropZoneType.PLAYER)
        {
            PhotonNetwork.RaiseEvent(SYNC_PLAYER_DISCARD, info, true, null);
        }
        else if (this.dropZoneType == DropZoneType.INFECTION)
        {
            Debug.Log("Raising correct event");

            PhotonNetwork.RaiseEvent(SYNC_INFECTION_DISCARD, info, true, null);
        }
    }

    private void Start()
    {
        usedCards = new List<GameObject>();
        usedCardCount = 0;
    }

    private void Awake()
    {
        PhotonNetwork.OnEventCall += OnEvent;
    }

    private void OnEvent(byte eventCode, object content, int senderId)
    {
        if ((eventCode == SYNC_PLAYER_DISCARD && dropZoneType == DropZoneType.PLAYER) || (eventCode == SYNC_INFECTION_DISCARD && dropZoneType == DropZoneType.INFECTION))
        {
            int childCount = this.transform.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                if (this.transform.GetChild(i).GetComponent<CardModel>() != null)
                    Destroy(this.transform.GetChild(i).gameObject);
            }

            usedCards.Clear();

            int[] cards = (int[])content;

            usedCardCount = cards.Length;

            usedCardCounter.text = usedCardCount.ToString();

            foreach (int card in cards)
            {
                GameObject cardGameObject = Instantiate<GameObject>(cardPrefab);

                CardModel cardModel = cardGameObject.GetComponent<CardModel>();
                cardModel.ShowCardFace(card);

                cardGameObject.transform.SetParent(this.transform);
                cardGameObject.transform.localPosition = new Vector3(28, 0);
                cardGameObject.transform.SetSiblingIndex(this.transform.childCount - 3);
                cardGameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;

                usedCards.Add(cardGameObject);
            }
        }
    }
}
