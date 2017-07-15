using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardModel : MonoBehaviour {

    public enum CardType { EPIDEMIC, PLAYER_CITY, INFECTION_CITY, EVENT };

    public List<Sprite> cardFaces;
    public List<CardType> cardTypes;
    public CardType cardType;

    Image image;

    public void ShowCardFace(int cardIndex)
    {
        image.sprite = cardFaces[cardIndex];
        cardType = cardTypes[cardIndex];
    }

    private void Awake()
    {
        image = GetComponent<Image>();
    }
}
