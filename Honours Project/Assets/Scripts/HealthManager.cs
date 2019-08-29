using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class HealthManager : MonoBehaviourPunCallbacks {


	[SerializeField] private float health = 100.0f;
	[SerializeField] private float maxHealth = 100.0f;
	public HealthManager referer;
	public float damageFactor = 1.0f;

	void Start() {
		maxHealth = health;
	}

	public float Health {
		get {
			return health;
		}
	}
	
	public float MaxHealth {
		get {
			return maxHealth;
		}
	}

	public void ApplyDamage(float damage) {
		if(IsDead) return;

		damage *= damageFactor;

		if(referer) {
			referer.ApplyDamage(damage);
		}
		else {
			health -= damage;

			if(health <= 0) {
				health = 0;
			}

			photonView.RPC("RPCSetHealth", Photon.Pun.RpcTarget.Others, health);
		}
	}

	public void SetHealth(float newHealth) {
		health = newHealth;
		photonView.RPC("RPCSetHealth", Photon.Pun.RpcTarget.Others, newHealth);
	}

	public void SetMaxHealth(float newHealth) {
		maxHealth = newHealth;
		photonView.RPC("RPCSetMaxHealth", Photon.Pun.RpcTarget.Others, newHealth);
	}

	public void SetDamageFactor(float newFactor) {
		damageFactor = newFactor;
		photonView.RPC("RPCSetDamageFactor", Photon.Pun.RpcTarget.Others, newFactor);
	}

	public void Heal() {
		SetHealth(maxHealth);
	}

	[PunRPC]
	void RPCSetHealth(float newHealth) {
		health = newHealth;
	}

	[PunRPC]
	void RPCSetMaxHealth(float newHealth) {
		maxHealth = newHealth;
	}

	[PunRPC]
	void RPCSetDamageFactor(float newFactor) {
		damageFactor = newFactor;
	}

	public bool IsDead {
		get {
			if(!referer) {
				return health <= 0;
			}
			else {
				return referer.IsDead;
			}
		}
	}
}
