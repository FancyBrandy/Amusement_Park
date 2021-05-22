using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Script for controlling the main camera
 */

public class BirdEyeCamera : MonoBehaviour
{
    //values for default camera position
    private float x;
    private float y = 0f;
    private float height = 40f;
    private float viewAngle = 60f;
    private float rotationAngle = 0f;
    private static Vector3 cameraPos;

    //used for implementing camera position and angle drag
    private Vector3 dragOrigin;
    private Vector3 posDiff;
    private float angleDragOrigin;
    private float angleDiff = 0f;

    //constraints of the camera position
    private float minHeight = 5f;
    private float maxHeight = 100f;
    private float minDistance = 0f;
    private float maxDistance = 300f;

    private BuildSystem buildSystem;
    private bool disableDrag = false;
    private bool disableCam = false;

    void Start()
    {
        //starting position of the main camera
        x = maxDistance / 2f;
        cameraPos = new Vector3(x, height, y);

        //initializing variables
        buildSystem = GameObject.FindObjectOfType<BuildSystem>();

        //move camera
        HandleCamera();
    }

    void Update()
    {
        if(!disableCam) //if no other scripts are disabling the main camera movement
            HandleCamera();
    }

    private void HandleCamera()
    {
        //Moving the camera using arrow keys
        Vector3 newPos = cameraPos;
        Quaternion rotationQuat = Quaternion.AngleAxis(rotationAngle, Vector3.up);
        float speed = Time.deltaTime * cameraPos.y * 1.5f;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            newPos += rotationQuat * Vector3.forward * speed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            newPos += rotationQuat * Vector3.back * speed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            newPos += rotationQuat * Vector3.right * speed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            newPos += rotationQuat * Vector3.left * speed;
        }
        ClampCamera(newPos);

        //Moving the camera using middle mouse button drag
        float dragSpeed = (cameraPos.y / 350f); // here a static speed is smoother
        if (Input.GetMouseButtonDown(2) && !disableDrag)
        {
            dragOrigin = Input.mousePosition;
            posDiff = cameraPos;
            return;
        }
        if (Input.GetMouseButton(2) && !disableDrag)
        {
            Vector3 mousePos = Input.mousePosition;
            float deltaX = (dragOrigin.x - mousePos.x) * dragSpeed;
            float deltaY = (dragOrigin.y - mousePos.y) * dragSpeed;
            
            newPos = posDiff + rotationQuat * (new Vector3( deltaX, 0f, deltaY));
            ClampCamera(newPos);
        }

        //Rotating the camera around the Z axis using right mouse button drag
        if (Input.GetMouseButtonDown(1))
        {
            angleDragOrigin = Input.mousePosition.x;
            angleDiff = rotationAngle;
            return;
        }
        if (Input.GetMouseButton(1))
        {
            float mousePos = Input.mousePosition.x;
            float deltaX = mousePos - angleDragOrigin;
            rotationAngle = (angleDiff + deltaX/5) % 360;
        }

        //Zooming in and out using mouse wheel/touch pad (might not work for macOS users)
        cameraPos.y = Mathf.Clamp(cameraPos.y + Input.mouseScrollDelta.y * -5, minHeight, maxHeight);

        //Actually moving the camera
        float interRatio = Time.deltaTime * 3f;
        transform.position = Vector3.Slerp(transform.position, cameraPos, interRatio);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(viewAngle, rotationAngle, 0f), interRatio);
    }

    private void ClampCamera(Vector3 newPos)//constrains the position of the camera to within the playable area
    {
        cameraPos.x = Mathf.Clamp(newPos.x, minDistance, maxDistance);
        cameraPos.z = Mathf.Clamp(newPos.z, minDistance, maxDistance);
    }

    public void toggleDragLock()
    {
        disableDrag = !disableDrag;
    }

    public void camLockState(bool b)
    {
        disableCam = b;
    }
}
