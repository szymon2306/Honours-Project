using UnityEngine;
using System.Collections;

public enum RotationAxes
{
    MouseXAndY = 0,
    MouseX = 1,
    MouseY = 2
}

public class MouseLook : MonoBehaviour
{
    public RotationAxes axes;
    public float sensitivity = 2;
    public float minimumX;
    public float maximumX;
    public float minimumY;
    public float maximumY;
    float rotationY;
	InGameMenu menu = null;
	
    void Start()
    {
      menu = InGameMenu.instance;
	  //sensitivity = menu.sensitivity;
      sensitivity = 2;
    }
	
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.None){
			sensitivity = menu.sensitivity;
			return;
		}
        
        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + (Input.GetAxis("Mouse X") * sensitivity);
            rotationY = rotationY + (Input.GetAxis("Mouse Y") * sensitivity);
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else
        {
            if (axes == RotationAxes.MouseX)
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
            }
            else
            {
                rotationY = rotationY + (Input.GetAxis("Mouse Y") * sensitivity);
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
                transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
            }
        }
    }
}