using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseLook : MonoBehaviour
{

    [SerializeField] private float mouseSensitivity = 100f;

    float xRotation = 0f;

    Transform player;

    bool follow;

    private void Start()
    {       
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player = transform.parent;
        follow = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            follow = !follow;
            Cursor.visible = !Cursor.visible;
            if (follow)
            {
                Cursor.lockState = CursorLockMode.Locked;
            } else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }


        if (follow)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.unscaledDeltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.unscaledDeltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            player.Rotate(Vector3.up * mouseX);
        }        
    }
}
