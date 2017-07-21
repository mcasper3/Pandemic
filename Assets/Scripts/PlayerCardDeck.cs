using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCardDeck : MonoBehaviour {
    public const int UPDATE_COUNTER = 150;

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

    private void Awake()
    {
        PhotonNetwork.OnEventCall += OnEvent;
    }

    private void OnEvent(byte eventCode, object content, int senderId)
    {
        if (eventCode == UPDATE_COUNTER)
        {
            numCards = (int)content;

            cardCount.text = numCards.ToString();
        }
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

        PhotonNetwork.RaiseEvent(UPDATE_COUNTER, numCards, true, null);

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

	public void AddPandemicCards()
    {
        cards.Insert(0, 53);
        cards.Insert(9, 54);
        cards.Insert(18, 55);
        cards.Insert(27, 56);
        cards.Insert(35, 57);
        cards.Insert(43, 58);
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
