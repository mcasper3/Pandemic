using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameInfo gameInfo;
    public CardDeck playerCardDeck;
    public List<PlayerHand> playerHands;

    private int currentPlayer;
    private Color currentPlayerDisabledColor;
    private bool isSecondCard;

    private List<string> roles;
    
    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        SceneManager.LoadScene("Main Menu");
    }

    public void OnPlayerCardClick()
    {
        if (!playerCardDeck.IsEmpty)
        {
            var card = playerCardDeck.DrawCard();

            var cardModel = card.GetComponent<CardModel>();

            if (cardModel.cardType == CardModel.CardType.EPIDEMIC)
            {
                // TODO increment infection rate
                gameInfo.UpdateInfectionRate();
                Destroy(card);
            }
            else
            {
                playerHands[currentPlayer].AddCard(card);
            }

            if (isSecondCard)
            {
                // TODO do infection step
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
        StartGame();
	}

    private void StartGame()
    {
        // TODO assign roles
        AssignRoles();

        playerCardDeck.CreateDeck();
        // TODO deal out x cards to each player (make sure to avoid epidemics)
        // for int i = 0; i < total number of cards for players
        // playerhand[i % 4].AddCard(card);
        SetFirstPlayer(0);

        // TODO figure out who will go first
    }

    private void AssignRoles()
    {
        roles = new List<string>(7);
        roles.Add("Medic");
        roles.Add("Dispatcher");
        roles.Add("Quarantine Specialist");
        roles.Add("Contingency Planner");
        roles.Add("Researcher");
        roles.Add("Scientist");
        roles.Add("Operations Expert");
    }

    private void SetFirstPlayer(int position)
    {
        currentPlayer = position;
        currentPlayerDisabledColor = playerHands[position].GetComponent<Image>().color;
        playerHands[position].GetComponent<Image>().color = new Color(currentPlayerDisabledColor.r, currentPlayerDisabledColor.g, currentPlayerDisabledColor.b);
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
