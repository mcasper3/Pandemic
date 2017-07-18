using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfectionCardDeck : MonoBehaviour {
    public Text cardCount;
    public GameObject cardPrefab;
    public DropZone.DropZoneType cardType;

    private List<int> cards;
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

        return cardGameObject;
    }

    public void Shuffle(int start, int end)
    {
        while (start > 1)
        {
            start--;

            int k = Random.Range(start, end + 1);
            int temp = cards[k];
            cards[k] = cards[start];
            cards[start] = temp;
        }
    }

    public GameObject DrawFromBottom()
    {
        int lastCardPosition = cards.Count - 1;
        int card = cards[lastCardPosition];

        cards.RemoveAt(lastCardPosition);
        numCards--;
        cardCount.text = numCards.ToString();

        GameObject cardGameObject = Instantiate<GameObject>(cardPrefab);

        CardModel cardModel = cardGameObject.GetComponent<CardModel>();
        cardModel.ShowCardFace(card);

        return cardGameObject;
    }
}
