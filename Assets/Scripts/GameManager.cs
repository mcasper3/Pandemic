using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public CardDeck playerCardDeck;
    public List<PlayerHand> playerHands;

    private int currentPlayer;
    
    public void OnPlayerCardClick()
    {
        if (!playerCardDeck.IsEmpty)
        {
            var card = playerCardDeck.DrawCard();

            var cardModel = card.GetComponent<CardModel>();

            if (cardModel.cardType == CardModel.CardType.EPIDEMIC)
            {
                // TODO increment epidemic count and potentially infection rate
            }
            else
            {
                playerHands[currentPlayer].AddCard(card);
            }
        }
        else
        {
            Debug.Log("Out of cards!");
        }
    }

	// Use this for initialization
	void Start () {
        currentPlayer = 0;
	}
}
