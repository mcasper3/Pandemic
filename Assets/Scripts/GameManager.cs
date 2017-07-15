using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public CardDeck playerCardDeck;
    public List<PlayerHand> playerHands;

    private int currentPlayer;
    private Color currentPlayerDisabledColor;
    private bool isSecondCard;
    
    public void OnPlayerCardClick()
    {
        if (!playerCardDeck.IsEmpty)
        {
            var card = playerCardDeck.DrawCard();

            var cardModel = card.GetComponent<CardModel>();

            if (cardModel.cardType == CardModel.CardType.EPIDEMIC)
            {
                // TODO increment epidemic count and potentially infection rate
                Destroy(card);
            }
            else
            {
                playerHands[currentPlayer].AddCard(card);
            }

            if (isSecondCard)
            {
                MoveToNextPlayer();
            }

            isSecondCard = !isSecondCard;
        }
        else
        {
            Debug.Log("Out of cards!");
        }
    }

	// Use this for initialization
	void Start () {
        currentPlayer = 0;
        currentPlayerDisabledColor = playerHands[0].GetComponent<Image>().color;
        playerHands[0].GetComponent<Image>().color = new Color(currentPlayerDisabledColor.r, currentPlayerDisabledColor.g, currentPlayerDisabledColor.b);
        isSecondCard = false;
	}

    private void MoveToNextPlayer()
    {
        playerHands[currentPlayer].GetComponent<Image>().color = currentPlayerDisabledColor;

        currentPlayer = (currentPlayer + 1) % 4;

        currentPlayerDisabledColor = playerHands[currentPlayer].GetComponent<Image>().color;

        playerHands[currentPlayer].GetComponent<Image>().color = new Color(currentPlayerDisabledColor.r, currentPlayerDisabledColor.g, currentPlayerDisabledColor.b);
    }
}
