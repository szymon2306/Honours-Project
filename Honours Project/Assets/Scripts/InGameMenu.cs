using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class InGameMenu : MonoBehaviour
{
    public static InGameMenu instance = null;

    public GameObject joinGamePanel;

    public TextMeshProUGUI playerNameLabel;
    public TextMeshProUGUI playerPingLabel;
    public TextMeshProUGUI playerKillsLabel;
    public TextMeshProUGUI playerDeathLabel;

    float _soundVolume = 1f;    

    public GameManager gameManager;

    [HideInInspector] public float sensitivity;
    [HideInInspector] public bool showMenu;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        showMenu = true;
    }
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M)) || Input.GetKeyDown(KeyCode.BackQuote))
        {
            showMenu = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (showMenu == true)
        {
            joinGamePanel.SetActive(true);
            PlayerList();
        }
        else
        {
            joinGamePanel.SetActive(false);
        }
    }

    public void CloseMenu()
    {
        showMenu = false;
    }

    public void JoinGameButton()
    {
        showMenu = false;
        // Spawn player
        gameManager.SpawnPlayer();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SettingsButton()
    {

    }

    public void ResumeGameButton()
    {
        showMenu = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void DisconnectButton()
    {
        PhotonNetwork.Disconnect();
        //networkManager.Disconnect();
    }


    public void LeaveCurrentRoomButton()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void PlayerList()
    {
        foreach (Player nPlayer in PhotonNetwork.PlayerList)
        {
            playerNameLabel.text = nPlayer.NickName;
            playerPingLabel.text = "PING: " + PhotonNetwork.GetPing().ToString();
            // player kills
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game

        // Disconnect player from Photon Server
        PhotonNetwork.Disconnect();
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("APPLICATION CLOSED / EXIT GAME SUCCESSFUL");
#else
                        // Disconnect player from Photon Server
                        PhotonNetwork.Disconnect();
                        Application.Quit();
#endif
    }
}
