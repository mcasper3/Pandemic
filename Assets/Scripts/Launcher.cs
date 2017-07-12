﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : Photon.PunBehaviour {

    public string version = "0.1";
    public PhotonLogLevel networkLogging = PhotonLogLevel.ErrorsOnly;

    private void Awake()
    {
        PhotonNetwork.logLevel = networkLogging;
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
    }

    public void MasterServerConnect()
    {
        Debug.Log("Trying to connect to master server");

        PhotonNetwork.ConnectUsingSettings(this.version);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected successfully");

        SceneManager.LoadScene("Connection");
    }

    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        string errorMessage;

        switch (cause)
        {
            case DisconnectCause.AuthenticationTicketExpired:
                errorMessage = "Old Authentication Ticket";
                break;
            case DisconnectCause.DisconnectByClientTimeout:
                errorMessage = "Client Timeout Disconnect";
                break;
            case DisconnectCause.DisconnectByServerLogic:
                errorMessage = "Server Logic Disconnect";
                break;
            case DisconnectCause.DisconnectByServerTimeout:
                errorMessage = "Server Timeout Disconnect";
                break;
            case DisconnectCause.DisconnectByServerUserLimit:
                errorMessage = "Server User Limit Disconnect";
                break;
            case DisconnectCause.Exception:
                errorMessage = "Exception Disconnect";
                break;
            case DisconnectCause.ExceptionOnConnect:
                errorMessage = "Local Server not Running";
                break;
            case DisconnectCause.InternalReceiveException:
                errorMessage = "Internal Receive Exception";
                break;
            case DisconnectCause.InvalidAuthentication:
                errorMessage = "Invalid Authentication";
                break;
            case DisconnectCause.InvalidRegion:
                errorMessage = "Invalid Region";
                break;
            case DisconnectCause.MaxCcuReached:
                errorMessage = "Max CCU";
                break;
            case DisconnectCause.SecurityExceptionOnConnect:
                errorMessage = "Security Exception on Connect";
                break;
            default:
                errorMessage = "Unknown Exception: " + Convert.ToInt32(cause).ToString();
                break;
        }

        Debug.Log(errorMessage);
    }

}
