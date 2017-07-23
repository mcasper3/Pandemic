﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfectionCardDeck : MonoBehaviour {
    public const int UPDATE_INFECTION_COUNTER = 180;

    public Text cardCount;
    public GameObject cardPrefab;
    public DropZone.DropZoneType cardType;

    public List<int> cards;
    private int numCards;

    public bool IsEmpty
    {
        get { return cards == null || cards.Count == 0; }
    }

    private void Start()
    {
        cards = new List<int>();
        numCards = 48;
        cardCount.text = "48";
    }

    private void Awake()
    {
        PhotonNetwork.OnEventCall += OnEvent;
    }

    private void OnEvent(byte eventCode, object content, int senderId)
    {
        if (eventCode == UPDATE_INFECTION_COUNTER)
        {
            numCards = (int)content;
            cardCount.text = numCards.ToString();
        }
    }

    public void CreateDeck()
    {
        for (int i = 0; i < 48; i++)
        {
            cards.Add(i);
        }

        Shuffle(0, cards.Count);
    }

    public void AddCardsToTop(List<GameObject> cardsToInsert)
    {
        int cardsAdded = cardsToInsert.Count;

        foreach (var card in cardsToInsert) {
            cards.Insert(0, card.GetComponent<CardModel>().cardPosition);
            Destroy(card);
        }

        numCards = cards.Count;

        Shuffle(0, cardsAdded);

        cardCount.text = numCards.ToString();

        Debug.Log(numCards + " is the number of cards");

        PhotonNetwork.RaiseEvent(UPDATE_INFECTION_COUNTER, numCards, true, null);
    }

    public GameObject DrawCard()
    {
        int card = cards[0];

        cards.RemoveAt(0);
        numCards--;
        cardCount.text = numCards.ToString();

        GameObject cardGameObject = Instantiate<GameObject>(cardPrefab);

        CardModel cardModel = cardGameObject.GetComponent<CardModel>();
        cardModel.ShowCardFace(card);

        PhotonNetwork.RaiseEvent(UPDATE_INFECTION_COUNTER, numCards, true, null);

        return cardGameObject;
    }

    public void Shuffle(int start, int end)
    {
        while (end > start + 1)
        {
            end--;

            int k = Random.Range(start, end + 1);
            int temp = cards[k];
            cards[k] = cards[end];
            cards[end] = temp;
        }
    }

    public GameObject DrawFromBottom()
    {
        int lastCardPosition = cards.Count - 1;
        int card = cards[lastCardPosition];

        cards.RemoveAt(lastCardPosition);
        numCards--;
        cardCount.text = numCards.ToString();

        PhotonNetwork.RaiseEvent(UPDATE_INFECTION_COUNTER, numCards, true, null);

        GameObject cardGameObject = Instantiate<GameObject>(cardPrefab);

        CardModel cardModel = cardGameObject.GetComponent<CardModel>();
        cardModel.ShowCardFace(card);

        return cardGameObject;
    }
}
