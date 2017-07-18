using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {
    private List<GameObject> cards;

    private void Start()
    {
        if (cards == null)
            cards = new List<GameObject>();
    }

    public IEnumerable<GameObject> GetCards()
    {
        foreach (var card in cards)
        {
            yield return card;
        }
    }

    public void AddCard(GameObject card)
    {
        if (cards == null)
            cards = new List<GameObject>();

        cards.Add(card);
        card.transform.SetParent(this.transform);
    }
}
