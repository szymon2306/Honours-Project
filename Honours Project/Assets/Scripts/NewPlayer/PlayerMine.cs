﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class PlayerMine : MonoBehaviourPunCallbacks {
	public GameObject inspectorPrefab;
	public GameObject indicatorUIPrefab;
	public AudioClip deathSound;

	CharacterController controller;
	HealthManager healthManager;
	Transform character;
	Transform rootRig;
	GameManager gameManager;
	AudioSource audioSource;
	bool isDestroyed = false;

	public int upgradeHealth = 0;
	public int upgradeRegeneration = 0;

	void Awake() {
		controller = GetComponent<CharacterController>();
		healthManager = GetComponent<HealthManager>();
		character = transform.Find("Character");
		rootRig = character.Find("Healthmale/Root");
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	void Start() {
		DeactivateRagdoll();
	}

	void Update() {
		if(healthManager.IsDead && !isDestroyed) {
			isDestroyed = true;

			if(photonView.IsMine) {
				DestroyAllIndicators();

				WeaponManager weaponManager = GetComponent<WeaponManager>();
				FundSystem fundSystem = GetComponent<FundSystem>();
				Transform fpsChar = transform.Find("FirstPersonCharacter");
				Transform weaponHolder = fpsChar.Find("WeaponHolder");
				int weaponsCount = weaponHolder.childCount;
				float totalFund = fundSystem.GetFund();

				// Add weapon cost
				switch(weaponManager.primaryWeapon) {
					case Weapon.AKM:
						totalFund += 850;
						break;
					case Weapon.M870:
						totalFund += 650;
						break;
					case Weapon.UMP45:
						totalFund += 450;
						break;
					case Weapon.MP5K:
						totalFund += 250;
						break;
				}

				switch(weaponManager.secondaryWeapon) {
					case Weapon.Python:
						totalFund += 300;
						break;
				}

				// Accumulate upgraded costs
				for(int i = 0; i < weaponsCount; i++) {
					Transform weapon = weaponHolder.GetChild(i);
					WeaponBase weaponBase = weapon.GetComponent<WeaponBase>();
					
					totalFund += weaponBase.upgradeSpent;
					weaponBase.upgradeSpent = 0;
				}

				// Accumulate other upgraded costs
				// Upgrade Health
				if(upgradeHealth > 1) {
					int cost = 100 + ((upgradeHealth - 1) * 90);
					totalFund += cost;
				}

				// Upgrade Regeneration
				if(upgradeRegeneration > 1) {
					int cost = 100 + ((upgradeRegeneration - 1) * 70);
					totalFund += cost;
				}

                gameManager.SaveFund(System.Convert.ToInt32(totalFund));
				
				fpsChar.gameObject.SetActive(false);
				character.gameObject.SetActive(true);
				
				GameObject.Find("UI/InGameUI/PlayerUI").SetActive(false);
				GameObject.Find("UI/InGameUI/InspectorUI").SetActive(true);

				DeployInspector();

                //gameManager.CheckPlayerDead();

				photonView.RPC("RPCDeployBody", RpcTarget.All);	
			}
		}
	}

	public void StartCreateIndicators() {
		photonView.RPC("RPCStartCreateIndicators", RpcTarget.All);
	}

	[PunRPC]
	void RPCStartCreateIndicators() {
		CreatePlayerIndicators();
	}

	void DestroyAllIndicators() {
		GameObject[] indicators = GameObject.FindGameObjectsWithTag("AllyIndicator");

		foreach(GameObject indicator in indicators) {
			indicator.GetComponent<AllyIndicator>().SetTarget(null);
		}
	}

	public void CreatePlayerIndicators() {
		if(!photonView.IsMine) return;

		DestroyAllIndicators();

		GameObject playerUI = GameObject.Find("UI/InGameUI/PlayerUI");
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
	
		foreach(GameObject player in players) {
			if(player == gameObject) continue;

			GameObject indicator = Instantiate(indicatorUIPrefab, Vector3.zero, Quaternion.identity);
			indicator.GetComponent<AllyIndicator>().SetTarget(player);
			indicator.transform.parent = playerUI.transform;
		}
	}

	void DeployInspector() {
		Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z - 1);
		Instantiate(inspectorPrefab, spawnPosition, transform.rotation);
	}

	[PunRPC]
	void RPCDeployBody() {
		character.gameObject.SetActive(true);
        //gameManager.ShowPlayerDead(GetComponent<NetworkPlayer>().playerName);

		character.GetComponent<SoundManager>().Play(deathSound);
		character.SetParent(null);
		ActivateRagdoll();

		if(photonView.IsMine) PhotonNetwork.Destroy(gameObject);
	}

	void ActivateRagdoll() {
		controller.enabled = false;
		character.GetComponent<Animator>().enabled = false;

		// Hide weapons
		character.Find("Glock").GetComponent<SkinnedMeshRenderer>().enabled = false;
		character.Find("Colt Python").GetComponent<SkinnedMeshRenderer>().enabled = false;
		character.Find("MP5K").GetComponent<SkinnedMeshRenderer>().enabled = false;
		character.Find("UMP45").GetComponent<SkinnedMeshRenderer>().enabled = false;
		character.Find("M870").GetComponent<SkinnedMeshRenderer>().enabled = false;
		character.Find("AKM").GetComponent<SkinnedMeshRenderer>().enabled = false;

		CharacterJoint[] joints = rootRig.GetComponentsInChildren<CharacterJoint>();
		Rigidbody[] rigidbodies = rootRig.GetComponentsInChildren<Rigidbody>();
		Collider[] colliders = rootRig.GetComponentsInChildren<Collider>();
		
		foreach(CharacterJoint joint in joints) {
			joint.enablePreprocessing = false;
			joint.enableProjection = true;
		}
		foreach(Rigidbody rigidbody in rigidbodies) {
			rigidbody.useGravity = true;
			rigidbody.isKinematic = false;
		}
		foreach(Collider collider in colliders) {
			collider.enabled = true;
		}
		
	}

	void DeactivateRagdoll() {
		Rigidbody[] rigidbodies = rootRig.GetComponentsInChildren<Rigidbody>();
		Collider[] colliders = rootRig.GetComponentsInChildren<Collider>();

		foreach(Rigidbody rigidbody in rigidbodies) {
			rigidbody.useGravity = false;
			rigidbody.isKinematic = true;
		}
		foreach(Collider collider in colliders) {
			collider.enabled = false;
		}
	}

	void DisableWeapon(WeaponBase weapon) {
		weapon.IsEnabled = false;
	}

	void DisableController(FPSController controller) {
		controller.enabled = false;
	}
	void OnControllerColliderHit(ControllerColliderHit hit) {
		if(hit.gameObject.tag == "BulletCase") {
			Physics.IgnoreCollision(GetComponent<Collider>(), hit.gameObject.GetComponent<Collider>());
		}
	}

	public void ActivateHealthRegeneration() {
		StartCoroutine(CoHealthRegeneration());
	}

	IEnumerator CoHealthRegeneration() {
		while(!healthManager.IsDead) {
			float nextDelay = 5f - (upgradeRegeneration * 0.35f);
			
			if(healthManager.Health < healthManager.MaxHealth) {
				healthManager.SetHealth(healthManager.Health + 1);
			}

			yield return new WaitForSeconds(nextDelay);
		}

		yield break;
	}
}
