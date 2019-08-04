using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;


public class MainMenuLauncher : MonoBehaviourPunCallbacks
{
    [Header("Login Panel")]
    public GameObject loginPanel;

    public TMP_InputField PlayerNameInput;
    public TMP_InputField ServerIPInput;
    public TMP_Dropdown NetworkModeDropdown;
    public TextMeshProUGUI connectionStatusText;
    private readonly string connectionStatusString = " ";

    [Header("Selection Panel")]
    public GameObject SelectionPanel;

    public Text ServerIP;

    [Header("Create Room Panel")]
    public GameObject CreateRoomPanel;

    public InputField RoomNameInputField;
    public InputField MaxPlayersInputField;
    public TMP_Dropdown SelectedRoomDropdown;

    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomPanel;

    [Header("Room List Panel")]
    public GameObject RoomListPanel;

    public GameObject RoomListContent;
    public GameObject RoomListEntryPrefab;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomPanel;

    public Button StartGameButton;

    NetworkManager networkManager;


    #region UI CALLBACKS

    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        SetActivePanel(SelectionPanel.name);
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = RoomNameInputField.text;
        roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

        byte maxPlayers;
        byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public void ChangeSelectedMap()
    {
        // logic for changing selected map from dropdown



    }

    public void OnJoinRandomRoomButtonClicked()
    {
        SetActivePanel(JoinRandomRoomPanel.name);

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;
        
        if (!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;

            networkManager.Connect();

            //PhotonNetwork.ConnectToMaster("127.0.0.1", 5055, MyAppID);

            //PhotonNetwork.ConnectToMaster("127.0.0.1", 5505, PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime);

            Debug.Log("Connected to Master Server");

            ServerIP.text = PhotonNetwork.ServerAddress;

        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }


    public void OnChangeNetworkModeDropdown()
    {

        if (NetworkModeDropdown.value == 0)
        {
            // online master server connection
            ServerIPInput.gameObject.SetActive(false);
        }

        if (NetworkModeDropdown.value == 1)
        {
            ServerIPInput.gameObject.SetActive(true);
        }

        else
        {
            ServerIPInput.gameObject.SetActive(false);
            // offline mode / solo play enabled
        }
    }

    public void OnPlayOfflineButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if (!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.OfflineMode = true;
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }


    public void OnRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        SetActivePanel(RoomListPanel.name);
    }

    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;


        PhotonNetwork.LoadLevel("Testing_Scene_01");

        if (SelectedRoomDropdown.Equals("Level_01"))
        {
            PhotonNetwork.LoadLevel("DemoAsteroids-GameScene");
        }

        //PhotonNetwork.LoadLevel("Testing_Scene_01");
        //PhotonNetwork.LoadLevel("DemoAsteroids-GameScene");
    }



    public void OnExitGameButtonClicked()
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

    #endregion

    private void SetActivePanel(string activePanel)
    {
        loginPanel.SetActive(activePanel.Equals(loginPanel.name));
        SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
        CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
        JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
        RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
        InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
    }


    public void Update()
    {
        // Update connection status text
        connectionStatusText.text = connectionStatusString + PhotonNetwork.NetworkClientState;
    }
}