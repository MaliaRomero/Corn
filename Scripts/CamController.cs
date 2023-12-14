using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [Header("Look Sensitivity")]
    public float sensX;
    public float sensY;

    public Transform orientation;


    [Header("Spectator")]
    //public float spectatorMoveSpeed;

    private float rotX;
    private float rotY;

    private bool isSpectator;

    // Start is called before the first frame update
    void Start()
    {
        // lock the cursor to the middle of the screen
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        // get the movement inputs
        float mousey = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        float mousex = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;

        rotY += mousex;
        rotX -= mousey;
        rotX = Mathf.Clamp(rotX, -90f, 90f);

            // rotate the camera vertically
        transform.rotation = Quaternion.Euler(rotX, rotY, 0);

            //rotate the player horizontally
        orientation.rotation = Quaternion.Euler(0, rotY, 0);
    }
}
