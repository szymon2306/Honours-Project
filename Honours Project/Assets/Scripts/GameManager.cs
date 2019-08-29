using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager _instance = null;

    public Camera overviewCameraStart;
    public AudioListener audioListener;

    public TextMeshProUGUI infoText;

    public Transform playerSpawnArea;

    int savedFund = 0;
    public int spawnedPlayers = 0;
    [SerializeField] int alives = 0;

    GameObject player;

    // Match start timer, set to 5 second by default
    public float matchStartTime = 5;

    [HideInInspector] public GameObject playerReference;

    bool changingMap = false;

    private void Awake()
    {
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
    }

    [PunRPC]
    void RPCSyncUserSpawned()
    {
        spawnedPlayers++;
        alives++;

        if (spawnedPlayers == PhotonNetwork.PlayerList.Length)
        {
            RefreshPlayerIndicators();
        }

        photonView.RPC("RPCSyncAlives", RpcTarget.Others, alives);
    }

    [PunRPC]
    void RPCSyncAlives(int alives)
    {
        this.alives = alives;
    }

    void RefreshPlayerIndicators()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerMine>().StartCreateIndicators();
        }
    }

    public void RevivePlayers()
    {
        RefreshPlayerIndicators();
        photonView.RPC("RPCRevivePlayers", RpcTarget.Others);
    }

    [PunRPC]
    void RPCRevivePlayers()
    {
        if (player == null)
        {
            Destroy(GameObject.FindWithTag("Inspector"));

            savedFund += (PhotonNetwork.PlayerList.Length * 100);

            SpawnPlayer();
            player.GetComponent<FundSystem>().AddFund(savedFund);

            LevelSystem levelSystem = player.GetComponent<LevelSystem>();

            savedFund = 0;
        }

        RefreshPlayerIndicators();
    }

    public void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        infoText.text = "";

        Hashtable props = new Hashtable
            {
                {LobbyPlayers.PLAYER_LOADED_LEVEL, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // run Spawn Player
        //SpawnPlayer();

        // Disable level start camera and audio listener
        //TurnStartCamera(false);

        // run Start Game
        //StartGame();
    }

    public void SaveFund(int amount)
    {
        savedFund = amount;
    }

    //Start Game Function
    private void StartGame()
    {
        // Start the countdown for match start
        if (PhotonNetwork.IsMasterClient)
        {
            infoText.text = "Waiting for other players...";
            StartCoroutine(StartGameCounter());
        }
    }

    private void RestartGame()
    {
        //Logic for restart game
    }

    //Turns On/Off level camera and audio listener
    void TurnStartCamera(bool state)
    {
        overviewCameraStart.enabled = audioListener.enabled = state;
    }

    public void SpawnPlayer()
    {
        if (playerReference != null) return;

        /* Spawn player prefab in spawn zone
        playerReference = PhotonNetwork.Instantiate(this._playerPrefab.name, GetSpawnZone(), Quaternion.identity, 0);
        */

        // Spawn player prefab in spawn zone
        playerReference = PhotonNetwork.Instantiate("Player", GetSpawnZone(), Quaternion.identity, 0);

        //Configure the local player
        playerReference.GetComponent<PlayerConfig>().LocalPlayer(); // enable scripts locally 

    }

    private IEnumerator StartGameCounter()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1, 5));


            // Start game logic and code

        }
    }    

    /// Returns a random spawn position within the team's spawn area.
    public Vector3 GetSpawnZone()
    {
        //init variables
        Vector3 pos = playerSpawnArea.position;
        BoxCollider col = playerSpawnArea.GetComponent<BoxCollider>();

        if (col != null)
        {
            // find a position within the box collider range, first set fixed y position
            // the counter determines how often we are calculating a new position if out of range
            // set Y axis of spawn position to be the center of spawn collider on its Y axis
            pos.y = col.bounds.center.y;

            int counter = 10;

            //try to get random position within collider bounds
            //if it's not within bounds, do another iteration
            do
            {
                pos.x = UnityEngine.Random.Range(col.bounds.min.x, col.bounds.max.x);
                pos.z = UnityEngine.Random.Range(col.bounds.min.z, col.bounds.max.z);
                counter--;
            }
            while (!col.bounds.Contains(pos) && counter > 0);
        }
        //return spawn position
        return pos;
    }

    private void OnCountdownTimerIsExpired()
    {
        StartGame();
    }

    public override void OnLeftRoom()
    {
        LeaveRoom();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        SceneManager.LoadScene("Main_Menu", LoadSceneMode.Single);
    }

    public void RemovePlayer()
    {
        changingMap = true;
        if (playerReference != null)
        {
            PhotonNetwork.Destroy(playerReference);
            TurnStartCamera(true);
        }
    }

}
