using UnityEngine;
using UnityEngine.UI;
//PUN2 Starter KIT [www.armedunity.com]

public class PlayerName : MonoBehaviour 
{
	public Transform usernameText;
	public Text textUI;
	Camera cam = null;
	float overDistance;
	float adjustSize = 0.002f;
	float hideUsernameDistance = 7f;
	
	public void SetName (string username){
		textUI.text = username;
		this.gameObject.SetActive(true);
		this.enabled = true;
	}
	
	void Update () 
	{
		if(cam){
			overDistance = Vector3.Distance(usernameText.position, cam.transform.position);	
			if(overDistance > hideUsernameDistance) transform.localScale = Vector3.zero;
			else{
				transform.localScale = Vector3.one * overDistance * adjustSize;
				usernameText.LookAt(usernameText.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
			}	
		}else cam = Camera.main;
	}
}
