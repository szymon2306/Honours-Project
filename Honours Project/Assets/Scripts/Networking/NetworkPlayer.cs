using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class NetworkPlayer : MonoBehaviourPunCallbacks {
	public GameObject localCam;
    public Animator animator;

    WeaponManager weaponManager;
    public Weapon currentWeapon;
    FPSController controller;
    HealthManager health;

    Dictionary<Weapon, GameObject> weapons = new Dictionary<Weapon, GameObject>();

    public string playerName;

    public bool IsLocalPlayer {
        get {
            return photonView.IsMine;
        }
    }

	void Awake() {
        weaponManager = GetComponent<WeaponManager>();
        weaponManager.isLocalPlayer = photonView.IsMine;

        health = GetComponent<HealthManager>();

        controller = GetComponent<FPSController>();
        
        if(!photonView.IsMine) {
            DisableScripts();

            weapons.Add(Weapon.Glock, transform.Find("Character/Glock").gameObject);
            weapons.Add(Weapon.Python, transform.Find("Character/Colt Python").gameObject);
            weapons.Add(Weapon.MP5K, transform.Find("Character/MP5K").gameObject);
            weapons.Add(Weapon.UMP45, transform.Find("Character/UMP45").gameObject);
            weapons.Add(Weapon.M870, transform.Find("Character/M870").gameObject);
            weapons.Add(Weapon.AKM, transform.Find("Character/AKM").gameObject);
        }
	}

    [PunRPC]
    void RPCSyncPlayerName(string name) {
        playerName = name;
    }
    
	void Update() {
        weaponManager.isLocalPlayer = photonView.IsMine;

        if(!photonView.IsMine) {

            UpdateWeapon();
            UpdateAnimator();
        }
        else {

            photonView.RPC("RPCSyncPlayerAnimValues", RpcTarget.Others, weaponManager.currentWeapon);
        }
    }

    void DisableScripts() {
        // LevelSystem levelSystem = GetComponentInChildren<LevelSystem>();
        // FundSystem fundSystem = GetComponentInChildren<FundSystem>();
        HealthManager healthManager = GetComponentInChildren<HealthManager>();
        UpdateHealth updateHealth = GetComponent<UpdateHealth>();
        WeaponBase[] weapons = GetComponentsInChildren<WeaponBase>();

        // levelSystem.enabled = false;
        // fundSystem.enabled = false;
        healthManager.enabled = false;
        updateHealth.enabled = false;

        foreach(WeaponBase weapon in weapons) {
            weapon.enabled = false;
        }
    }

	public void SwitchWeapon() {
        photonView.RPC("TriggerLocalWeaponSwitch", RpcTarget.Others);
	}

    public void FireWeapon() {
        photonView.RPC("TriggerLocalWeaponFire", RpcTarget.Others);
    }

    public void ReloadWeapon() {
        photonView.RPC("TriggerLocalWeaponReload", RpcTarget.Others);
    }

    [PunRPC]
    void TriggerLocalWeaponSwitch() {
        animator.SetTrigger("Switch_Weapon");
    }

    [PunRPC]
    void TriggerLocalWeaponFire() {
        animator.SetTrigger("Fire");
    }

    [PunRPC]
    void TriggerLocalWeaponReload() {
        animator.SetTrigger("Reload");
    }

    [PunRPC]
    void RPCSyncPlayerAnimValues(Weapon weapon) {
        currentWeapon = weapon;
    }

    void UpdateWeapon() {
        Weapon[] weaponKeys = new Weapon[weapons.Keys.Count];
        weapons.Keys.CopyTo(weaponKeys, 0);

        for(int i = 0; i < weaponKeys.Length; i++) {
            if(weaponKeys[i] == currentWeapon) {
                weapons[weaponKeys[i]].SetActive(true);
            }
            else {
                weapons[weaponKeys[i]].SetActive(false);
            }
        }
    }

    void UpdateAnimator() {
		switch(currentWeapon) {
			case Weapon.Glock:
				animator.SetBool("Weapon_Glock", true);
				animator.SetBool("Weapon_MP5K", false);
                animator.SetBool("Weapon_Python", false);
                animator.SetBool("Weapon_UMP45", false);
				animator.SetBool("Weapon_M870", false);
				animator.SetBool("Weapon_AKM", false);
				break;
			case Weapon.MP5K:
				animator.SetBool("Weapon_Glock", false);
				animator.SetBool("Weapon_MP5K", true);
                animator.SetBool("Weapon_Python", false);
                animator.SetBool("Weapon_UMP45", false);
				animator.SetBool("Weapon_M870", false);
				animator.SetBool("Weapon_AKM", false);
				break;
            case Weapon.Python:
				animator.SetBool("Weapon_Glock", false);
				animator.SetBool("Weapon_MP5K", false);
                animator.SetBool("Weapon_Python", true);
                animator.SetBool("Weapon_UMP45", false);
				animator.SetBool("Weapon_M870", false);
				animator.SetBool("Weapon_AKM", false);
				break;
            case Weapon.UMP45:
				animator.SetBool("Weapon_Glock", false);
				animator.SetBool("Weapon_MP5K", false);
                animator.SetBool("Weapon_Python", false);
                animator.SetBool("Weapon_UMP45", true);
				animator.SetBool("Weapon_M870", false);
				animator.SetBool("Weapon_AKM", false);
				break;
			case Weapon.M870:
				animator.SetBool("Weapon_Glock", false);
				animator.SetBool("Weapon_MP5K", false);
                animator.SetBool("Weapon_Python", false);
                animator.SetBool("Weapon_UMP45", false);
				animator.SetBool("Weapon_M870", true);
				animator.SetBool("Weapon_AKM", false);
				break;
			case Weapon.AKM:
				animator.SetBool("Weapon_Glock", false);
				animator.SetBool("Weapon_MP5K", false);
                animator.SetBool("Weapon_Python", false);
                animator.SetBool("Weapon_UMP45", false);
				animator.SetBool("Weapon_M870", false);
				animator.SetBool("Weapon_AKM", true);
				break;
		}
	}
}
