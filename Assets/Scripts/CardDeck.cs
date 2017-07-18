using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDeck : MonoBehaviour
{
    public Text cardCount;
    public GameObject cardPrefab;
    public DropZone.DropZoneType cardType;

    private List<int> cards;
    private int numCards;

    public bool IsEmpty
    {
        get { return cards == null || cards.Count == 0; }
    }

    void Start()
    {
        cards = new List<int>();

        if (cardCount != null)
            cardCount.text = numCards.ToString();

        numCards = cardType == DropZone.DropZoneType.PLAYER ? 59 : 48;
        // TODO remove
        numCards = 10;

        if (this.GetComponent<DropZone>() != null)
            numCards = 0;

        this.CreateDeck();
    }

    public IEnumerable<int> GetCards()
    {
        foreach (int i in cards)
        {
            yield return i;
        }
    }

    public void CreateDeck()
    {
        if (cardCount != null)
            cardCount.text = numCards.ToString();

       cards.Clear();

        for (int i = 0; i < numCards; i++)
        {
            cards.Add(i);
        }

        Shuffle();
    }

    public void Shuffle()
    {
        int n = cards.Count;

        while (n > 1)
        {
            n--;

            int k = Random.Range(0, n + 1);
            int temp = cards[k];
            cards[k] = cards[n];
            cards[n] = temp;
        }
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

    public void AddCardsToTop(IEnumerable<int> cardsToInsert)
    {
        cards.InsertRange(0, cardsToInsert);
        numCards = cards.Count;
    }

    public void AddCardToTop(int card)
    {
        cards.Insert(0, card);
        numCards++;
    }

    public void AddCard(int card)
    {
        cards.Add(card);
        numCards++;
    }
}
