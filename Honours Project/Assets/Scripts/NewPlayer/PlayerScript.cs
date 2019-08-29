using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
	public string playerName = "";

	public int currentPlayerHealth;
	public float maxPlayerHealth = 100;

	public int currentPlayerStamina;
	public float maxPlayerStamina = 100;	

	public int playerLevel = 1;

	public int playerMoney = 0;

	public bool playerIsAlive = true;

	public string currentWeapon = "";





    // Start is called before the first frame update
    void Start()
    {
		// Set player variables / Override on start
		playerIsAlive = true;
		playerLevel = 1;
		playerMoney = 0;
		currentPlayerHealth = 100; // TESTING
		currentPlayerStamina = 100; // TESTING
	}

    // Update is called once per frame
    void Update()
    {
        // Player Death

		if(currentPlayerHealth <= 0)
		{
			playerIsAlive = false;
			PlayerDeath();
		}
    }

	public void PlayerDeath()
	{
		//
		// Player Death
		//
		
		// Play player Death sound
		//script

		// Play player death animation
		//script

		// Destroy Player Game Object
		Destroy(gameObject, 3);

		// Display Death Screen
	}

	public void OnPlayerLevelUp()
	{

	}

	public void UpdatePlayerHealth()
	{

	}



}
