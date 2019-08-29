using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartupText : MonoBehaviour {
	Text text;
	int remainSeconds;
	
	public AudioClip prepare;
	public AudioClip gameBegins;
	public AudioClip beep;


	void OnEnable() {
		text = GetComponent<Text>();
		remainSeconds = 15;

		StartCoroutine(StartAnimation());
	}

	IEnumerator StartAnimation() {
		for(int i = remainSeconds; i > 0; i--) {
			if(i <= 5) {
                // play start sound
			}

			UpdateText(i);
			yield return new WaitForSeconds(1f);
		}

		text.text = "FIGHT!";

		yield return new WaitForSeconds(3f);

		gameObject.SetActive(false);
	}

	void UpdateText(int sec) {
		text.text = "Prepare to fight...\nBegins at " + sec + " seconds.";
	}
}
