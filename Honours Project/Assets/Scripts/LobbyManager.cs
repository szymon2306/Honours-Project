using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
//PUN2 Starter KIT [www.armedunity.com]

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Lobby Properties")]
    public string roomName = "";
    public int maxPlayers;
    public int playerCount;
    public bool isVisible;
    public bool isOpen;

    public string mapName = "";

    void Start()
    {
        roomName = (string)PhotonNetwork.CurrentRoom.Name; // room name
        maxPlayers = (int)PhotonNetwork.CurrentRoom.MaxPlayers; //limit of players in room
        playerCount = (int)PhotonNetwork.CurrentRoom.PlayerCount; //count of players in room
        isVisible = (bool)PhotonNetwork.CurrentRoom.IsVisible; //if this room is listed
        isOpen = (bool)PhotonNetwork.CurrentRoom.IsOpen; //if players can join this room

        // current map
        mapName = (string)PhotonNetwork.CurrentRoom.CustomProperties["map"];
    }

    // Called when a room’s custom properties have changed 
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {



    }
}
