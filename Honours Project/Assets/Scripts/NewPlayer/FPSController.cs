using UnityEngine;
using System.Collections;

public class FPSController : MonoBehaviour
{
    public int crouchSpeed = 3;
    public int walkSpeed = 7;
    public int runSpeed = 12;
    public float jumpSpeed = 8.0f;
    public float gravity = 20f;
    public float fallingDamageThreshold = 8f;
	
    private float _slideSpeed = 8.0f;
    private float _antiBumpFactor = .75f;
    private float _antiBunnyHopFactor = 1f;
    public bool airControl = false;
	public Transform cameraGO;
	
	Vector3 moveDirection = Vector3.zero;
    [HideInInspector] public bool run;
	[HideInInspector] public bool grounded = false;
    [HideInInspector] public float speed;
	[HideInInspector] public int state = 0;
	
    private RaycastHit hit;
    private float fallDistance;
    private bool falling = false;
	private float slideLimit = 45.0f;
    private float rayDistance;
    private int jumpTimer;
    private float normalHeight = 0.9f;
    private float crouchHeight = 0.2f;

    private Vector3 currentPosition = Vector3.zero;
    private Vector3 lastPosition = Vector3.zero;
    private float highestPoint;

    private float crouchProneSpeed = 3f;
    private float distanceToObstacle;

    private bool sliding = false;
    [HideInInspector] public float velMagnitude;

    public CharacterController controller;
    public Animation cameraAnimations;
	public bool playRunAnimation = false;
    private string runAnimation = "Run";
    private string idleAnimation = "Idle";
    public Transform fallEffect;
	float returnSpeed = 3.0f;

    void Start() 
	{
        rayDistance = controller.height / 2 + 1.1f;
        slideLimit = controller.slopeLimit - .2f;
        cameraAnimations[runAnimation].speed = 0.8f;
    }

    void Update()
    {
        velMagnitude = controller.velocity.magnitude;
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f) ? .7071f : 1.0f;

        if (grounded)
        {
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, rayDistance))
            {
                float hitangle = Vector3.Angle(hit.normal, Vector3.up);
                if (hitangle > slideLimit)
                {
                    sliding = true;
                }
                else
                {
                    sliding = false;
                }
            }

            if (state == 0)
            {
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
                {
                    run = true;
                }
                else
                {
                    run = false;
                }
            }

            if (falling)
            {
                falling = false;
                fallDistance = highestPoint - currentPosition.y;
                if (fallDistance > fallingDamageThreshold)
                {
                    ApplyFallingDamage(fallDistance);
                }

                if (fallDistance > 0.5f)
                {
                    StartCoroutine(FallCamera(new Vector3(12, Random.Range(-3.0f, 3.0f), 0), new Vector3(3, Random.Range(-1f, 1f), 0), 0.1f));
                }
            }

            if (sliding)
            {
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize( ref hitNormal, ref moveDirection);
                moveDirection *= _slideSpeed;
            }
            else
            {
                if (state == 0)
                {
                    if (run)
                        speed = runSpeed;
                    else if (Input.GetMouseButton(1))
                    {
                        speed = crouchSpeed;
                    }
                    else
                    {
                        speed = walkSpeed;
                    }
                }
                else if (state == 1)
                {
                    speed = crouchSpeed;
                    run = false;
                }
				
                if (Cursor.lockState == CursorLockMode.Locked)
                    moveDirection = new Vector3(inputX * inputModifyFactor, -_antiBumpFactor, inputY * inputModifyFactor);
                else
                    moveDirection = new Vector3(0, -_antiBumpFactor, 0);

				moveDirection = transform.TransformDirection(moveDirection);
				moveDirection *= speed;

                if (!Input.GetKey(KeyCode.Space))
                {
                    jumpTimer++;
                }
                else if (jumpTimer >= _antiBunnyHopFactor)
                {
                    jumpTimer = 0;
                    if (state == 0)
                    {
                        moveDirection.y = jumpSpeed;
                    }

                    if (state > 0)
                    {
                        CheckDistance();
                        if (distanceToObstacle > 1.6f)
                        {
                            state = 0;
							controller.height = 2.0f;
							controller.center = Vector3.zero;
                        }
                    }
                }
            }
        }
        else
        {
            currentPosition = transform.position;
            if (currentPosition.y > lastPosition.y)
            {
                highestPoint = transform.position.y;
                falling = true;
            }

            if (!falling)
            {
                highestPoint = transform.position.y;
                falling = true;
            }

            if (airControl)
            {
                moveDirection.x = inputX * speed;
                moveDirection.z = inputY * speed;
                moveDirection = transform.TransformDirection(moveDirection);
            }
        }

        if (grounded)
        {
            if (run && velMagnitude > walkSpeed && playRunAnimation)
            {
                cameraAnimations.CrossFade(runAnimation);
            }
            else
            {
                cameraAnimations.CrossFade(idleAnimation);
            }
        }
        else
        {
            cameraAnimations.CrossFade(idleAnimation);
            run = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CheckDistance();

            if (state == 0)
            {
                state = 1;
				controller.height = 1.4f;
				controller.center = new Vector3(0, -0.3f, 0);
            }
            else if (state == 1)
            {
                if (distanceToObstacle > 1.6f)
                {
                    state = 0;
					controller.height = 2.0f;
					controller.center = Vector3.zero;
                }
            }
        }

        if (state == 0) //Stand Position
        { 
            if (cameraGO.localPosition.y > normalHeight)
            {
                cameraGO.localPosition = new Vector3(cameraGO.localPosition.x, normalHeight, cameraGO.localPosition.z);
            }
            else if (cameraGO.localPosition.y < normalHeight)
            {
                cameraGO.localPosition = new Vector3(cameraGO.localPosition.x,cameraGO.localPosition.y + Time.deltaTime * crouchProneSpeed, cameraGO.localPosition.z) ;
            }

        }
        else if (state == 1) //Crouch Position
        { 
            if (cameraGO.localPosition.y != crouchHeight)
            {
                if (cameraGO.localPosition.y > crouchHeight)
                {
                    cameraGO.localPosition = new Vector3(cameraGO.localPosition.x,cameraGO.localPosition.y - Time.deltaTime * crouchProneSpeed, cameraGO.localPosition.z) ;
                }
                if (cameraGO.localPosition.y < crouchHeight)
                {
                    cameraGO.localPosition = new Vector3(cameraGO.localPosition.x, cameraGO.localPosition.y + Time.deltaTime * crouchProneSpeed, cameraGO.localPosition.z) ;
                }
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }

    void CheckDistance()
    {
        Vector3 pos = transform.position + controller.center - new Vector3(0, controller.height / 2, 0);
        RaycastHit hit;
        if (Physics.SphereCast(pos, controller.radius, transform.up, out hit, 10))
        {
            distanceToObstacle = hit.distance;
        }
        else
        {
            distanceToObstacle = 3;
        }
    }

    void LateUpdate()
    {
        lastPosition = currentPosition;
		fallEffect.localRotation = Quaternion.Slerp(fallEffect.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
    }

    void ApplyFallingDamage(float fallDistance)
    {
		
    }
	
    IEnumerator FallCamera(Vector3 d, Vector3 dw, float ta)
    {
        Quaternion s = fallEffect.localRotation;
        Quaternion e = fallEffect.localRotation * Quaternion.Euler(d);
		
        float r = 1.0f / ta;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * r;
            fallEffect.localRotation = Quaternion.Slerp(s, e, t);
            yield return null;
        }
    }
}