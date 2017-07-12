using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Connection : MonoBehaviour {
    private const string HAS_JOINED = " has joined!";

    public Text playerOneName;
    public Text playerTwoName;
    public Text playerThreeName;
    public Text playerFourName;

    public void Start()
    {
        if (PhotonNetwork.playerList.Length == 4)
        {
            // TODO start game
        }
        else
        {
            UpdatePlayerList();
        }
    }

    private void UpdatePlayerList()
    {
        var numPlayers = PhotonNetwork.playerList.Length;
        var players = PhotonNetwork.playerList;

        if (numPlayers > 2)
        {
                playerThreeName.text = players[2].NickName + HAS_JOINED;
        }

        if (numPlayers > 1)
        {
                playerTwoName.text = players[1].NickName + HAS_JOINED;
        }

        if (numPlayers > 0)
        {
            playerOneName.text = players[0].NickName + HAS_JOINED;
        }
    }
}
