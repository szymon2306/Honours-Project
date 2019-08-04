using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class MasterServerConnection : MonoBehaviour
{
    // readonly string of connection status
    private readonly string connectionStatusMessage = " ";

    [Header("UI References")]
    public TextMeshProUGUI ConnectionStatusText;

    #region UNITY

    // Update connection status text
    public void Update()
    {
        ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
    }

    #endregion
}
