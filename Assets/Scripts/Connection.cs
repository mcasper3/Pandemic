using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Connection : Photon.PunBehaviour {
    private const string HAS_JOINED = " has joined!";

    public Text playerOneName;
    public Text playerTwoName;
    public Text playerThreeName;
    public Text playerFourName;

    public void Start()
    {
        UpdatePlayerList();
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Player connected");

        if (PhotonNetwork.room.PlayerCount == 2 && PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
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
