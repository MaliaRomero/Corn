using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public bool isSpectator;
    public float rotSpeed;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(inputDir != Vector3.zero)
        {
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotSpeed);

        }

    }
    /*
    if(isSpectator)
        {
            transform.rotation = Quaternion.Euler(-rotY, rotX, 0);

            float x = Input.GetAxis("Horizontal");
    float z = Input.GetAxis("Vertical");
    float y = 0;

            if (Input.GetKey(KeyCode.E))
                y = 1;
            else if (Input.GetKey(KeyCode.Q))
                y = -1;

            Vector3 dir = transform.right * x + transform.up * y + transform.forward * z;
    transform.position += dir* spectatorMoveSpeed * Time.deltaTime;
        }

    */

}
