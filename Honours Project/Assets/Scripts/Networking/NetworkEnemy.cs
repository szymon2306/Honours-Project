using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class NetworkEnemy : MonoBehaviourPunCallbacks {
	Chasing chasing;
	HealthManager healthManager;
	Animator animator;
	AudioSource audioSource;
	public AudioClip attackSound;
	public AudioClip deathSound;
	bool wasAlreadyDead = false;
	Vector3 oldPos = Vector3.zero;
    Quaternion oldRot = Quaternion.identity;
	Vector3 syncPos;
	Quaternion syncRot;

	void Awake() {
		animator = GetComponent<Animator>();
		healthManager = GetComponent<HealthManager>();
		chasing = GetComponent<Chasing>();
		audioSource = GetComponent<AudioSource>();

		syncPos = transform.position;
		syncRot = transform.rotation;
	}

	void Start() {

	}

	void Update() {
		if(wasAlreadyDead == true) return;

		if(PhotonNetwork.IsMasterClient) {
			CheckSyncPosition();
			CheckSyncRotation();
		}
		else {
			transform.position = Vector3.Lerp(transform.position, syncPos, 0.1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, syncRot, 0.1f);
		}

		// Stop chasing and set dead if it's dead
		if(!wasAlreadyDead && healthManager.IsDead) {
			wasAlreadyDead = true;

			SetDead();
			chasing.StopChasing();
		}
	}

	void CheckSyncPosition() {
		if(oldPos != transform.position) {
			photonView.RPC("RPCSyncPosition", Photon.Pun.RpcTarget.All, transform.position);
		}
	}

	void CheckSyncRotation() {
		if(oldRot != transform.rotation) {
			photonView.RPC("RPCSyncRotation", Photon.Pun.RpcTarget.All, transform.rotation);
		}
	}

	[PunRPC]
	void RPCSyncPosition(Vector3 pos) {
		syncPos = pos;
	}

	[PunRPC]
	void RPCSyncRotation(Quaternion rot) {
		syncRot = rot;
	}

	[PunRPC]
	void RPCTriggerDeadAnimation() {
		if(wasAlreadyDead) return;

		animator.SetTrigger("Dead");
		audioSource.PlayOneShot(deathSound);
	}

	[PunRPC]
	void RPCTriggerAttackAnimation() {
		animator.SetTrigger("Attack");
		audioSource.PlayOneShot(attackSound);
	}

	public void TriggerAttackAnimation() {
		animator.SetTrigger("Attack");
		audioSource.PlayOneShot(attackSound);

		photonView.RPC("RPCTriggerAttackAnimation", Photon.Pun.RpcTarget.Others);
	}

	public void SetDead() {
		RemoveColliders(GetComponents<Collider>());
		RemoveColliders(GetComponentsInChildren<Collider>());

		animator.SetTrigger("Dead");
		audioSource.PlayOneShot(deathSound);
		
		photonView.RPC("RPCTriggerDeadAnimation", Photon.Pun.RpcTarget.Others);

		if(photonView.IsMine) {
			StartCoroutine(RemoveGameObject());
		}
	}

	void RemoveColliders(Collider[] colliders) {
		foreach(Collider collider in colliders) {
			collider.enabled = false;
		}
	}

	IEnumerator RemoveGameObject() {
		yield return new WaitForSeconds(5f);
		PhotonNetwork.Destroy(gameObject);
	}
}
