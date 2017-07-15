using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardDeck : MonoBehaviour {
    const int NUM_CARDS = 6;

    public Text cardCount;
    public GameObject cardPrefab;

    private List<int> cards;
    private int numCards;

    public bool IsEmpty
    {
        get { return cards == null || cards.Count == 0; }
    }

    void Start()
    {
        cards = new List<int>();
        numCards = NUM_CARDS;
        cardCount.text = numCards.ToString();
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
       cards.Clear();

        for (int i = 0; i < NUM_CARDS; i++)
        {
            cards.Add(i);
        }

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
}
