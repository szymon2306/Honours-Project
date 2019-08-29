using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class FundSystem : MonoBehaviourPunCallbacks {
	[SerializeField] private int fund = 0;

	Text fundText;
	RewardText rewardText;

	void Start() {
		fundText = GameObject.Find("UI/InGameUI/PlayerUI/CharacterStatus/FundText").GetComponent<Text>();
		rewardText = GetComponent<RewardText>();
		UpdateUI();
	}

	void UpdateUI() {
		if(photonView.IsMine && fundText != null) fundText.text = "Fund: " + fund.ToString() + " $";
	}

	public int GetFund() {
		return fund;
	}

	public void AddFund(int amount) {
		fund += amount;
		UpdateUI();

		photonView.RPC("RPCAddFund", RpcTarget.Others, amount);
	}

	[PunRPC]
	void RPCAddFund(int amount) {
		fund += amount;

		UpdateUI();
	}

	public void AddBonus(int exp, int amount) {
		fund += amount;
		UpdateUI();

		photonView.RPC("RPCAddBonus", RpcTarget.Others, exp, amount);
	}
	
	[PunRPC]
	void RPCAddBonus(int exp, int amount) {
		fund += amount;
		
		UpdateUI();
		rewardText.ShowBonus(exp, amount);
	}

	public void TakeFund(int amount) {
		fund -= amount;
		UpdateUI();

		photonView.RPC("RPCTakeFund", RpcTarget.Others, amount);
	}

	[PunRPC]
	void RPCTakeFund(int amount) {
		fund -= amount;
	}
}
