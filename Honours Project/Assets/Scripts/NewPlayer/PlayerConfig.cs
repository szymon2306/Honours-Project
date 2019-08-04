using System;
using UnityEngine;
using System.Collections;

public class PlayerConfig : MonoBehaviour
{
    public MonoBehaviour[] enableScipts;
    public GameObject enable;
	public GameObject disable;
	
	// called from GameManager (only on local client)
	// this way we can enable/disable scripts/objects we need on local player
    public void LocalPlayer()
    {
		for (int i = 0; i < enableScipts.Length; i++)	
        {
            enableScipts[i].enabled = true;
        }
        enable.SetActive(true);
		disable.SetActive(false);
    }
}




