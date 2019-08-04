using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ChangePlayerColors : MonoBehaviour 
{
	public PhotonView pv;
	public PlayerName playerName;
	public Renderer eyeRenderer;
	public Renderer bodyRenderer;

	void Start () 
	{
		if(pv.IsMine) return;

		Player owner = pv.Owner;
		playerName.SetName(owner.NickName);
		
		#if UNITY_EDITOR
			gameObject.name = owner.NickName;
		#endif
	
		int eyeIndex = (int)owner.CustomProperties["ec"];
		int bodyIndex = (int)owner.CustomProperties["bc"];

		eyeRenderer.material.SetColor("_Color", NetworkManager.colors[eyeIndex]);	
		bodyRenderer.material.SetColor("_Color", NetworkManager.colors[bodyIndex]);	
	}	
}
