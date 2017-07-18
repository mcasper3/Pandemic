using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCardDeck : MonoBehaviour {
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
        numCards = 59;
    }

    public void CreateDeck()
    {
        for (int i = 0; i < 53; i++)
        {
            cards.Add(i);
        }

        Shuffle(0, cards.Count);
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

	public void AddPandemicCards()
    {
        cards.Insert(0, 53);
        cards.Insert(10, 54);
        cards.Insert(19, 55);
        cards.Insert(28, 56);
        cards.Insert(36, 57);
        cards.Insert(44, 58);
    }

    public void ShuffleSections()
    {
        Shuffle(0, 8);
        Shuffle(9, 17);
        Shuffle(18, 26);
        Shuffle(27, 34);
        Shuffle(35, 42);
        Shuffle(43, 50);
    }
}
