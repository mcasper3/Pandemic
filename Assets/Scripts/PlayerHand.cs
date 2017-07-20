using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {
    public List<GameObject> cards;

    private void Start()
    {
        if (cards == null)
            cards = new List<GameObject>();
    }

    public List<GameObject> GetCards()
    {
        return cards;
    }

    public void ClearHand()
    {
        int cardCount = cards.Count;

        for (int i = 0; i < cardCount; i++)
        {
            Destroy(cards[0]);
            cards.RemoveAt(0);
        }
    }

    public void RemoveCard(GameObject card)
    {
        if (cards == null)
            return;

        cards.Remove(card);
    }

    public void AddCard(GameObject card)
    {
        if (cards == null)
            cards = new List<GameObject>();

        cards.Add(card);
        card.transform.SetParent(this.transform);
    }
}
