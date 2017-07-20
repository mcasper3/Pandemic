using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    // 1..49 for this class's events
    public const int CREATE_BLUE = 1;
    public const int CREATE_BLACK = 2;
    public const int CREATE_RED = 3;
    public const int CREATE_YELLOW = 4;
    public const int CREATE_RESEARCH = 5;
    public const int DESTROY_CUBE = 6;

    public const int DRAW_PLAYER_CARD = 20;
    public const int DRAW_INFECTION_CARD = 21;
    public const int SYNC_DISCARD_AND_HANDS = 22;
    public const int SYNC_PLAYER_HAND = 23;
    public const int READD_CITIES = 24;

    public GameInfo gameInfo;
    public PlayerCardDeck playerCardDeck;
    public InfectionCardDeck infectionCardDeck;
    public DropZone infectionDiscardPile;
    public List<PlayerHand> playerHands;
    public GameObject cubePrefab;
    public GameObject playerCardPrefab;

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
        PhotonNetwork.Disconnect();
    }

    public void OnEpidemic()
    {
        gameInfo.UpdateEpidemicCounter();
    }

    public void OnPlayerCardClick()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (!playerCardDeck.IsEmpty)
            {
                var card = playerCardDeck.DrawCard();

                var cardModel = card.GetComponent<CardModel>();

                if (cardModel.cardType == CardModel.CardType.EPIDEMIC)
                {
                    var infectedCity = infectionCardDeck.DrawFromBottom();

                    infectedCity.transform.SetParent(infectionDiscardPile.transform);
                    infectedCity.transform.SetSiblingIndex(infectedCity.transform.GetSiblingIndex() - 2);
                    infectedCity.transform.localPosition = new Vector3(28, 0);
                    infectionDiscardPile.usedCards.Add(infectedCity);

                    infectionDiscardPile.SyncDiscard();

                    gameInfo.UpdateInfectionRate();
                    Destroy(card);
                }
                else
                {
                    playerHands[currentPlayer].AddCard(card);

                    List<GameObject> cardsInHand = playerHands[currentPlayer].GetCards();

                    int[] info = new int[1 + cardsInHand.Count];

                    info[0] = currentPlayer;
                    int i = 1;

                    foreach (var c in cardsInHand)
                    {
                        info[i++] = c.GetComponent<CardModel>().cardPosition;
                    }

                    PhotonNetwork.RaiseEvent(SYNC_PLAYER_HAND, info, true, null);
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
        else
        {
            PhotonNetwork.RaiseEvent(DRAW_PLAYER_CARD, null, true, null);
        }
    }

    public void OnInfectionCardClick()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (!infectionCardDeck.IsEmpty)
            {
                var infectedCity = infectionCardDeck.DrawCard();

                infectedCity.transform.SetParent(infectionDiscardPile.transform);
                infectedCity.transform.SetSiblingIndex(infectedCity.transform.GetSiblingIndex() - 2);
                infectedCity.transform.localPosition = new Vector3(28, 0);
                infectionDiscardPile.usedCards.Add(infectedCity);

                Debug.Log("syncing...");

                infectionDiscardPile.SyncDiscard();
            }
            else
            {
                Debug.Log("Out of cards");
            }
        }
        else
        {
            PhotonNetwork.RaiseEvent(DRAW_INFECTION_CARD, null, true, null);
        }
    }

    private void OnEvent(byte eventCode, object content, int senderId)
    {
        Debug.Log("Received Event: " + eventCode);

        if (eventCode <= DESTROY_CUBE)
        {
            float[] info = (float[])content;

            Vector3 position = new Vector3(info[0], info[1], info[2]);

            switch (eventCode)
            {
                case CREATE_BLACK:
                    CreateCube(CubeDraggable.CubeColor.BLACK, position);
                    break;
                case CREATE_BLUE:
                    CreateCube(CubeDraggable.CubeColor.BLUE, position);
                    break;
                case CREATE_RED:
                    CreateCube(CubeDraggable.CubeColor.RED, position);
                    break;
                case CREATE_YELLOW:
                    CreateCube(CubeDraggable.CubeColor.YELLOW, position);
                    break;
                case CREATE_RESEARCH:
                    CreateCube(CubeDraggable.CubeColor.RESEARCH, position);
                    break;
                case DESTROY_CUBE:
                    DestroyCube(position);
                    break;
            }
        }
        else if (eventCode == DRAW_PLAYER_CARD)
        {
            if (PhotonNetwork.isMasterClient)
                OnPlayerCardClick();
        }
        else if (eventCode == SYNC_PLAYER_HAND)
        {
            int[] info = (int[])content;
            int player = info[0];

            playerHands[player].ClearHand();

            for (int i = 1; i < info.Length; i++)
            {
                AddPlayerCard(player, info[i]);
            }
        }
        else if (eventCode == SYNC_DISCARD_AND_HANDS)
        {
            SyncAllHands();
        }
        else if (eventCode == DRAW_INFECTION_CARD)
        {
            if (PhotonNetwork.isMasterClient)
                OnInfectionCardClick();
        }
        else if (eventCode == READD_CITIES)
        {
            ReaddCitiesForInfection();
        }
    }

    private void AddPlayerCard(int hand, int card)
    {
        GameObject cardGameObject = Instantiate<GameObject>(playerCardPrefab);

        CardModel cardModel = cardGameObject.GetComponent<CardModel>();
        cardModel.ShowCardFace(card);

        playerHands[hand].AddCard(cardGameObject);
    }

    public void HandleRightClick(BaseEventData data)
    {
        PointerEventData pointerEventData = data as PointerEventData;

        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            if (PhotonNetwork.isMasterClient)
            {
                ReaddCitiesForInfection();
            }
            else
            {
                PhotonNetwork.RaiseEvent(READD_CITIES, null, true, null);
            }
        }
    }

    private void ReaddCitiesForInfection()
    {
        infectionCardDeck.AddCardsToTop(infectionDiscardPile.GetUsedCards());
        infectionDiscardPile.ClearUsedCards();
        infectionDiscardPile.SyncDiscard();
    }

    public void CreateCube(CubeDraggable.CubeColor color, Vector3 position)
    {
        Debug.Log("Creating cube of color: " + color.ToString());

        GameObject go = Instantiate<GameObject>(cubePrefab);

        var image = go.GetComponent<Image>();

        switch (color)
        {
            case CubeDraggable.CubeColor.BLACK:
                image.color = new Color(0, 0, 0);
                break;
            case CubeDraggable.CubeColor.BLUE:
                image.color = new Color(0, 0, 255);
                break;
            case CubeDraggable.CubeColor.RED:
                image.color = new Color(255, 0, 0);
                break;
            case CubeDraggable.CubeColor.YELLOW:
                image.color = new Color(255, 237, 0);
                break;
            case CubeDraggable.CubeColor.RESEARCH:
                image.color = new Color(255, 255, 255);

                var rectTransform = go.GetComponent<RectTransform>();
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 25);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 15);

                break;
        }

        go.GetComponent<CubeDraggable>().isStockCube = false;

        GameObject parent = GameObject.Find("Map");
        go.transform.SetParent(parent.transform);

        go.transform.position = position;
    }

    public void DestroyCube(Vector3 position)
    {
        var objects = GameObject.FindObjectsOfType<GameObject>();

        foreach (var go in objects)
        {
            var objPos = go.transform.position;

            if (Mathf.Approximately(objPos.x, position.x) && Mathf.Approximately(objPos.y, position.y) && Mathf.Approximately(objPos.z, position.z) && go.GetComponent<CubeDraggable>() != null)
            {
                Destroy(go);
            }
        }
    }

    // Use this for initialization
    void Start () {
        if (PhotonNetwork.isMasterClient)
        {
            StartGame();
        }
	}

    private void Awake()
    {
        PhotonNetwork.OnEventCall += OnEvent;
    }

    private void StartGame()
    {
        // TODO assign roles
        AssignRoles();

        playerCardDeck.CreateDeck();

        DealCards();

        playerCardDeck.AddPandemicCards();
        playerCardDeck.ShuffleSections();

        infectionCardDeck.CreateDeck();

        DetermineFirstPlayer();

        // TODO raise event stating order of deck / cards and such
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

        SyncAllHands();
    }

    public void SyncAllHandsAndDiscard()
    {
        SyncAllHands();
    }

    private void SyncAllHands()
    {
        for (int i = 0; i < 4; i++)
        {
            var cardsInHand = playerHands[i].GetCards();
            int[] info = new int[1 + cardsInHand.Count];
            info[0] = i;
            int j = 1;

            foreach (var card in cardsInHand)
            {
                info[j++] = card.GetComponent<CardModel>().cardPosition;
            }

            PhotonNetwork.RaiseEvent(SYNC_PLAYER_HAND, info, true, null);
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
