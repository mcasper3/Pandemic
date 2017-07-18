using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameInfo gameInfo;
    public PlayerCardDeck playerCardDeck;
    public InfectionCardDeck infectionCardDeck;
    public DropZone infectionDiscardPile;
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

    public void OnEpidemic()
    {
        gameInfo.UpdateEpidemicCounter();
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

    public void OnCityDiscardClicked()
    {
        infectionCardDeck.AddCardsToTop(infectionDiscardPile.GetUsedCards());
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

        DealCards();

        playerCardDeck.AddPandemicCards();
        playerCardDeck.ShuffleSections();

        DetermineFirstPlayer();
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

    private void DealCards()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                playerHands[j].AddCard(playerCardDeck.DrawCard());
            }
        }
    }

    private void DetermineFirstPlayer()
    {
        int largestPoulation = 0;
        int playerIndex = 0;
        int currentPopulation;

        for (int i = 0; i < 4; i++)
        {
            foreach (var card in playerHands[i].GetCards())
            {
                currentPopulation = card.GetComponent<CardModel>().population;

                if (currentPopulation > largestPoulation)
                {
                    playerIndex = i;
                    largestPoulation = currentPopulation;
                }
            }
        }

        SetFirstPlayer(playerIndex);
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
