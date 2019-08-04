using UnityEngine;
using System.Collections;

public class FootSteps : MonoBehaviour 
{
	public AudioSource aSource;
    public AudioClip[] concrete;
    public AudioClip[] grass;
    public AudioClip[] metal;

    private float audioStepLengthCrouch = 0.75f;
    private float audioStepLengthWalk = 0.4f;
    private float audioStepLengthRun = 0.25f;
    private float minWalkSpeed = 3f;
    private float maxWalkSpeed = 6.0f;
    private float audioVolumeCrouch = 0.02f;
    private float audioVolumeWalk = 0.06f;
    private float audioVolumeRun = 0.15f;
    
	Vector3 previous = Vector3.zero;
	float velocity, speed, stepTime = 0f;

	void Update()
	{
		velocity = ((transform.position - previous).magnitude) / Time.deltaTime;
		speed = Mathf.Lerp(speed, velocity, Time.deltaTime * 10f);
		previous = transform.position;	
		
		stepTime -= Time.deltaTime;
		if(speed > 1 && stepTime <= 0.0f) CheckGround();
	}

	void CheckGround()
	{
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.4f, -transform.up, out hit, 0.7f)){

            if (hit.collider.CompareTag("Grass"))
            {
                if (speed > maxWalkSpeed) PlaySound(grass[Random.Range(0, grass.Length)], audioVolumeRun, audioStepLengthRun);
                else if (speed < maxWalkSpeed && speed > minWalkSpeed) PlaySound(grass[Random.Range(0, grass.Length)], audioVolumeWalk, audioStepLengthWalk);
                else if (speed < minWalkSpeed && speed > 0.5f) PlaySound(grass[Random.Range(0, grass.Length)], audioVolumeCrouch, audioStepLengthCrouch);
            }
            else if (hit.collider.CompareTag("Metal"))
            {
                if (speed > maxWalkSpeed) PlaySound(metal[Random.Range(0, metal.Length)], audioVolumeRun, audioStepLengthRun);
                else if (speed < maxWalkSpeed && speed > minWalkSpeed) PlaySound(metal[Random.Range(0, metal.Length)], audioVolumeWalk, audioStepLengthWalk);
                else if (speed < minWalkSpeed && speed > 0.5f) PlaySound(metal[Random.Range(0, metal.Length)], audioVolumeCrouch, audioStepLengthCrouch);
				
            } else {
				
                if (speed > maxWalkSpeed) PlaySound(concrete[Random.Range(0, concrete.Length)], audioVolumeRun, audioStepLengthRun);
                else if (speed < maxWalkSpeed && speed > minWalkSpeed) PlaySound(concrete[Random.Range(0, concrete.Length)], audioVolumeWalk, audioStepLengthWalk);
                else if (speed < minWalkSpeed && speed > 0.5f) PlaySound(concrete[Random.Range(0, concrete.Length)], audioVolumeCrouch, audioStepLengthCrouch);
            }				
        }
    }
	
	void PlaySound(AudioClip sound, float vol, float step)
	{
		aSource.PlayOneShot(sound, vol);
		stepTime = step;
	}
}