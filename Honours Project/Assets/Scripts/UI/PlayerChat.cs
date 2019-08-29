using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class PlayerChat : MonoBehaviourPunCallbacks
{

    public int maxMessages = 10;
    public float hideChatTime = 7f;
    public int messageCharacterLimit = 300;
    public GUISkin chatGUI;
    public InGameMenu menu;

    List<string> messages = new List<string>();
    string inputField = "";
    Vector2 scrollPosition;
    float fadeTime = 0f;
    bool showChat = false;
    bool showMessages = false;

    public override void OnPlayerEnteredRoom(Player nPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("Chat", RpcTarget.All, "<b>" + nPlayer.NickName + " joined game </b>", "");
        }
    }

    public override void OnPlayerLeftRoom(Player nPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("Chat", RpcTarget.All, nPlayer.NickName + " left game", "");
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("Chat", RpcTarget.All, "NEW MASTER PLAYER: " + newMasterClient.NickName, "");
        }
    }

    void OnGUI()
    {
        GUI.skin = chatGUI;
        GUI.color = new Color(1, 1, 1, fadeTime);
        scrollPosition.y = Mathf.Infinity;

        if (showChat)
        {
            GUILayout.BeginArea(new Rect(10, 10, 450, 252), "");
            GUILayout.BeginHorizontal(chatGUI.customStyles[0]);
            GUILayout.Label("<size=16> CHAT </size>");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(" X "))
            {
                CloseChat();
            }

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                CloseChat();
            }

            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginArea(new Rect(10, 45, 450, 182), "");
        }

        if (showMessages)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, "box");
            GUILayout.FlexibleSpace();

            for (int u = 0; u < messages.Count; u++)
            {
                GUILayout.Label(messages[u].ToString());
            }

            GUILayout.EndScrollView();
        }

        if (showChat)
        {
            GUILayout.BeginHorizontal("box");
            GUI.SetNextControlName("ChatField");
            inputField = GUILayout.TextField(inputField, messageCharacterLimit);
            GUI.FocusControl("ChatField");
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();

        if (Event.current.type == EventType.KeyDown && Event.current.character == '\n')
        {
            if (showChat)
            {
                if (inputField.Length == 0)
                {
                    CloseChat();
                }
            }
            else
            {
                fadeTime = hideChatTime;
                showMessages = true;
                showChat = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if (inputField.Length > 0)
            {
                showChat = false;
                photonView.RPC("Chat", RpcTarget.All, inputField, PhotonNetwork.LocalPlayer.NickName);
                inputField = "";
                if (!menu.showMenu)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
    }

    void CloseChat()
    {
        inputField = "";
        showChat = false;
        fadeTime = hideChatTime;
        showMessages = true;

        if (!menu.showMenu)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    [PunRPC]
    void Chat(string message, string senderName)
    {

        string nSender = "";

        if (senderName != null)
        {
            if (senderName == "")
            {
                nSender = "<b><color=orange>[GAME]</color></b>";
            }
            else
            {
                nSender = "<b><color=lime>" + senderName + "</color></b>";
            }
        }

        if (messages.Count > maxMessages) messages.RemoveAt(0);

        messages.Add(nSender + ": " + "<b>" + message + "</b>");

        fadeTime = hideChatTime;
        showMessages = true;
    }

    void Update()
    {
        if (!showChat && showMessages)
        {
            fadeTime -= Time.deltaTime;
            if (fadeTime <= 0.0f)
            {
                showMessages = false;
            }
        }
    }
}