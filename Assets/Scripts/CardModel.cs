using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardModel : MonoBehaviour {

    public List<Sprite> cardFaces;

    Image image;

    public void ShowCardFace(int cardIndex)
    {
        image.sprite = cardFaces[cardIndex];
    }

    private void Awake()
    {
        image = GetComponent<Image>();
    }
}
