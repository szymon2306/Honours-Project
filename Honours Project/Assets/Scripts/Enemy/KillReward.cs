using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class KillReward : MonoBehaviourPunCallbacks
{
	public int exp;
	public int fund;

	void Start() {
		photonView.RPC("RPCSetReward", Photon.Pun.RpcTarget.Others, exp, fund);
	}

	public void SetReward(int newExp, int newFund) {
		exp = newExp;
		fund = newFund;

		photonView.RPC("RPCSetReward", Photon.Pun.RpcTarget.Others, exp, fund);
	}

	[PunRPC]
	void RPCSetReward(int newExp, int newFund) {
		exp = newExp;
		fund = newFund;
	}
}
