using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public CardDeck playerCards;
    public List<PlayerHand> playerHands;

    private int currentPlayer;
    
    public void OnPlayerCardClick()
    {
        if (!playerCards.IsEmpty)
        {
            playerHands[currentPlayer].AddCard(playerCards.DrawCard());
        } else
        {
            Debug.Log("Out of cards!");
        }
    }

	// Use this for initialization
	void Start () {
        currentPlayer = 0;
	}
}
