using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

namespace Photon.Pun.Demo.Asteroids
{
    public class PlayerSpawn : MonoBehaviourPunCallbacks
    {
        public GameObject PlayerPrefab;

        private Dictionary<int, GameObject> playerListEntries;

        #region UNITY

        public void Awake()
        {
            playerListEntries = new Dictionary<int, GameObject>();

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject entry = Instantiate(PlayerPrefab);
                entry.transform.SetParent(gameObject.transform);
                entry.transform.localScale = Vector3.one;

                playerListEntries.Add(p.ActorNumber, entry);
            }
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherPlayer.ActorNumber);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            GameObject entry;
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                //Player info ie. health, name, etc. to be updated
            }
        }

        #endregion
    }
}