using UnityEngine;
using System.Collections;
using Photon.Pun;

public class PhotonNetworkSync : MonoBehaviour, IPunObservable 
{
	public float smoothPosition = 5f;
	public float smoothRotation = 10f;
	public PhotonView pv;
	public FPSController controls;
	public Animation anim;
	
	bool setPos = false;
    private Vector3 correctPlayerPos = Vector3.zero;
    private Quaternion correctPlayerRot = Quaternion.identity;
	private int playerState;
	
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
			stream.SendNext(controls.state);
        }
        else
        {
            correctPlayerPos = (Vector3) stream.ReceiveNext();
            correctPlayerRot = (Quaternion) stream.ReceiveNext();
			playerState = (int) stream.ReceiveNext();
			
			if(!setPos) SetSpawnPosition(correctPlayerPos);
        }
    }

    void Update()
    {
        if(!pv.IsMine && setPos)
		{
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * smoothPosition);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * smoothRotation);
			
			if(playerState == 1) anim.CrossFade("Crouch", 0.15f);
			else anim.CrossFade("Stand", 0.15f);
		}	
    }
	
	void SetSpawnPosition(Vector3 correctPos)
	{
		if(Vector3.Distance(transform.position, correctPos) > 1.0f ){
			transform.position = correctPos;
		}
		setPos = true;
	}
}