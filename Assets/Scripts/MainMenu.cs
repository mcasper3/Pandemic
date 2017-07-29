using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public const string PLAYER_NAME = "playerPrefs_playerName";

    public InputField userName;

	public void OnStartNewGame()
    {
        SceneManager.LoadScene("Connection");
    }

    public void OnResume()
    {
        // TODO
    }

    public void OnExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

    public void OnUpdateName()
    {
        Debug.Log("Selected Name: " + userName.text);

        PhotonNetwork.player.NickName = userName.text + " ";

        PlayerPrefs.SetString("name", userName.text);
    }
}
