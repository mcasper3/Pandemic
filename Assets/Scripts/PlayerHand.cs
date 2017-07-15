using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {
    private List<GameObject> cards;

    public void AddCard(GameObject card)
    {
        card.transform.SetParent(this.transform);
    }
}
