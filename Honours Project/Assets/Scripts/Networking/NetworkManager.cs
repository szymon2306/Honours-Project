using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("UI Elements")]
    [Header("Login Panel")]
    public GameObject loginPanel;

    public TMP_InputField playerNameInput;
    public TextMeshProUGUI playerNameText;
    public TMP_InputField serverIPInput;
    public TMP_Dropdown networkModeDropdown;
    public TextMeshProUGUI connectionStatusText;
    private readonly string connectionStatusString = " ";

    [Header("Selection Panel")]
    public GameObject selectionPanel;

    public Text serverIP;

    [Header("Create Room Panel")]
    public GameObject createRoomPanel;

    public TMP_InputField roomNameInput;
    public TMP_Dropdown maxPlayersDropdown;
    public TMP_Dropdown selectedMapDropdown;

    [Header("Join Random Room Panel")]
    public GameObject joinRandomRoomPanel;

    [Header("Room List Panel")]
    public GameObject roomListPanel;

    public GameObject roomListContent;
    public GameObject roomListEntryPrefab;

    [Header("Inside Room Panel")]
    public GameObject insideRoomPanel;

    public Button startGameButton;
    public GameObject playerListEntryPrefab;


    // Reference to this script instance
    private static NetworkManager _instance;

    // individual userID 
    public string userId { get; set; }

    // player ready string
    public const string playerReady = "IsPlayerReady";

    // Debug mode
    public bool debug = false;

    // Sync scenes
    public bool syncScenes = true;

    [Space(5f)]
    [Header("Network Management")]
    // Current version of the app
    public string appVersion = "0.1";

    // Maximum number of players for each room
    // Could be increased if players can spectate, etc
    //[Range(0, 4)]
    public int[] maxPlayers;
	int players = 2;

	public string[] gameLevelList;
	int gameLevelIndex;

    // Connection status
    public string connectionStatus = "";

    // Lobby related dictionaries:
    // Cached room list
    // Room list
    // Player list
    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;

    [Header("Player Character Customization")]
    public GameObject player;

    public Renderer eyeRenderer;
    static int eyeIndex = 0;

    public Renderer bodyRenderer;
    static int bodyIndex = 1;

    public static string[] colorList = new string[] { "Black", "White", "Red", "Green", "Blue" };
    public static Color[] colors = new Color[] { new Color(0.09f, 0.09f, 0.09f, 1), new Color(1, 1, 1, 1), new Color(1, 0.14f, 0.14f, 1), new Color(0, 1, 0, 1), new Color(0, 0.67f, 1, 1) };

	public TextMeshProUGUI playerNumberText;
	public TextMeshProUGUI gameLevelText;


	[Header("Room Info UI Reference")]
	public TextMeshProUGUI roomNameText;
	public TextMeshProUGUI maxPlayersText;
	public TextMeshProUGUI selectedMapText;



    // Initialize network view on Awake
    private void Awake()
    {
		/*
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

		*/

		// Set login panel to be active on awake in case it would not be for any reason
		loginPanel.SetActive(true);

        playerNameText.text = "";

        // Add a view to this gameobject with a unique viewID
        // This ensures that there is no same ID in the scene
        PhotonView view = gameObject.AddComponent<PhotonView>();
        view.ViewID = 999;

        // Sync all scenes
        PhotonNetwork.AutomaticallySyncScene = syncScenes; // true

        // Create new dictionary for
        // Cached room list
        // Room List
        // Player List
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            playerNameInput.text = "user" + Random.Range(1000, 10000); //made-up username
        }
    }

    // Return a reference to the instance of this script
    public static NetworkManager GetInstance()
    {
        return _instance;
    }

    private void SetActivePanel(string activePanel)
    {
        loginPanel.SetActive(activePanel.Equals(loginPanel.name));
        selectionPanel.SetActive(activePanel.Equals(selectionPanel.name));
        createRoomPanel.SetActive(activePanel.Equals(createRoomPanel.name));
        joinRandomRoomPanel.SetActive(activePanel.Equals(joinRandomRoomPanel.name));
        roomListPanel.SetActive(activePanel.Equals(roomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
        insideRoomPanel.SetActive(activePanel.Equals(insideRoomPanel.name));
    }

    public void Update()
    {
        // Update connection status text
        connectionStatusText.text = connectionStatusString + PhotonNetwork.NetworkClientState;
    }


    #region PUN CALLBACKS

    // Connect to master server
    public void Connect()
    {
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = this.userId;

        PhotonNetwork.ConnectUsingSettings();

        playerNameText.text = PhotonNetwork.LocalPlayer.NickName;
    }

    public void OnUserIdSubmited(string userId)
    {
        this.userId = userId;
        this.Connect();
    }

    public override void OnConnectedToMaster()
    {
        this.SetActivePanel(selectionPanel.name);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();

        ClearRoomListView();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(selectionPanel.name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(selectionPanel.name);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room " + Random.Range(1000, 10000);

        RoomOptions options = new RoomOptions { MaxPlayers = 4 };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public void ReConnect()
    {
        //this.connectionPanel.gameObject.SetActive(false);
        //this.advancedConnectionPanel.gameObject.SetActive(false);

        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = this.userId;

        //this.connectingLabel.SetActive(true);

        PhotonNetwork.Reconnect();
    }

    public void ReconnectAndRejoin()
    {
        //this.ConnectionPanel.gameObject.SetActive(false);
        //this.AdvancedConnectionPanel.gameObject.SetActive(false);

        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = this.userId;

        //this.ConnectingLabel.SetActive(true);

        PhotonNetwork.ReconnectAndRejoin();
    }

    public void ConnectOffline()
    {
        if (debug) Debug.Log("NetworkManager: ConnectOffline()");
        PhotonNetwork.OfflineMode = true;
    }

    public void JoinLobby()
    {
        if (debug) Debug.Log("NetworkManager: JoinLobby()");
        bool _result = PhotonNetwork.JoinLobby();

        if (!_result)
        {
            Debug.LogError("NetworkManager: Could not joinLobby");
        }

    }

    public void Disconnect()
    {
        if (debug) Debug.Log("NetworkManager: Disconnect()");
        PhotonNetwork.Disconnect();
    }

    public override void OnJoinedRoom()
    {
        SetActivePanel(insideRoomPanel.name);

		// Update Lobby Info once player joined room
		roomNameText.text = PhotonNetwork.CurrentRoom.Name;
		maxPlayersText.text = PhotonNetwork.CurrentRoom.PlayerCount + " of " + PhotonNetwork.CurrentRoom.MaxPlayers;
		selectedMapText.text = gameLevelList[gameLevelIndex];

		if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(playerListEntryPrefab);
            entry.transform.SetParent(insideRoomPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(playerReady, out isPlayerReady))
            {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            }

            playerListEntries.Add(p.ActorNumber, entry);
        }

        startGameButton.gameObject.SetActive(CheckPlayersReady());

        Hashtable props = new Hashtable
            {
                {LobbyPlayers.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnLeftRoom()
    {
        SetActivePanel(selectionPanel.name);

        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject entry = Instantiate(playerListEntryPrefab);
        entry.transform.SetParent(insideRoomPanel.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListEntries.Add(newPlayer.ActorNumber, entry);

        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);

        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            startGameButton.gameObject.SetActive(CheckPlayersReady());
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(playerReady, out isPlayerReady))
            {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            }
        }

        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    #endregion


    #region Lobby Callbacks

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(playerReady, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    private void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }

    public void LocalPlayerPropertiesUpdated()
    {
        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    // Update the view of all rooms in the list
    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(roomListEntryPrefab);
            entry.transform.SetParent(roomListContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

            roomListEntries.Add(info.Name, entry);
        }
    }

    #endregion


    #region UI CALLBACKS

    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        SetActivePanel(selectionPanel.name);
    }

    public void OnCreateRoomButtonClicked()
    {
		//roomNameInput.text = "Room" + Random.Range(1000, 10000); //made-up room name
		string roomName = roomNameInput.text;
        roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;		

		// New room settings
		RoomOptions newRoomOptions = new RoomOptions();
		newRoomOptions.IsOpen = true;
		newRoomOptions.IsVisible = true;

		newRoomOptions.MaxPlayers = (byte)maxPlayers[players];

		// Custom Room Properties
		newRoomOptions.CustomRoomProperties = new Hashtable();
		newRoomOptions.CustomRoomProperties.Add("gameLevel", gameLevelList[gameLevelIndex]);

		PhotonNetwork.CreateRoom(roomName, newRoomOptions, null);
	}

	public void ChangeMaxPlayers(int index)
	{
		playerNumberText.text = maxPlayers[index].ToString(); // Testing purposes
		players = index;

		//PhotonNetwork.CurrentRoom.MaxPlayers

		Debug.Log("Max players in this lobby is now: " + players);

	}

	public void ChangeSelectedMap(int index)
    {
		// logic for changing selected map from dropdown

		gameLevelText.text = gameLevelList[index]; // Testing purposes
		gameLevelIndex = index;

	}

    public void OnJoinRandomRoomButtonClicked()
    {
        SetActivePanel(joinRandomRoomPanel.name);

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;

        if (!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;

            Connect();

            //PhotonNetwork.ConnectToMaster("127.0.0.1", 5055, MyAppID);

            //PhotonNetwork.ConnectToMaster("127.0.0.1", 5505, PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime);

            Debug.Log("Connected to Master Server");

            serverIP.text = PhotonNetwork.ServerAddress;

        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }

    public void OnCustomiseButtonClicked()
    {

    }

    public void Customise()
    {

    }


    public void OnChangeNetworkModeDropdown()
    {

        if (networkModeDropdown.value == 0)
        {
            // online master server connection
            serverIPInput.gameObject.SetActive(false);
        }

        if (networkModeDropdown.value == 1)
        {
            serverIPInput.gameObject.SetActive(true);
        }

        else
        {
            serverIPInput.gameObject.SetActive(false);
            // offline mode / solo play enabled
        }
    }

    public void OnPlayOfflineButtonClicked()
    {
        string playerName = playerNameInput.text;

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

        SetActivePanel(roomListPanel.name);
    }

    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.IsVisible = true;

		PhotonNetwork.LoadLevel(gameLevelList[gameLevelIndex]);

		// Load testing scene level
		//PhotonNetwork.LoadLevel("Testing_Scene_01");
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
}
