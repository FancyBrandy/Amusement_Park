using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Script for being able to select certain characters and go into first person view, controlling them.
 */
public class Selectable : MonoBehaviour
{
    GameObject mainCam;//main camera in the scene
    GameObject playerCamObj;//Camera GameObject of the character the script is attached to
    Camera playerCam;//camera component of playerCamObj
    Animator anim;//animator of the character the script is attached to

    Vector3 targetPos;//position of playerCamObject in the scene
    Quaternion targetRot;//rotation of playerCamObject
    float mouseSensitivity = 200f;//sensitivity of the first person view
    float xRotation = 0f;//used to keep track of the vertical angle and constraining it

    bool selected;
    Vector3 mainCamOriginalPos;
    Quaternion mainCamOriginalRotation;
    BirdEyeCamera camScript;//used to disable main camera script
    GameObject buildingSystem;//used to hide ui buttons

    void Start()
    {
        //Main camera info
        mainCam = GameObject.Find("/Main Camera");
        mainCamOriginalPos = mainCam.transform.position;
        mainCamOriginalRotation = mainCam.transform.rotation;
        camScript = mainCam.GetComponent<BirdEyeCamera>();
        
        //Character camera info
        playerCamObj = gameObject.transform.Find("Camera").gameObject;
        playerCam = playerCamObj.GetComponent<Camera>();
        anim = GetComponent<Animator>();
        
        //initializing other variables
        buildingSystem = GameObject.Find("/BuildingSystem");
        selected = false;
    }

    void Update()
    {
        if (selected)
        {
            moveCamera(mainCam, targetPos, targetRot);

            if(mainCam.transform.position.y - targetPos.y < 0.1f)//when interpolation reaches close enough, just snap to the selected character
            {
                mainCam.SetActive(false);
                playerCamObj.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;

                movePlayer();
            }

            if (Input.GetKey(KeyCode.F1))//pressing F1 will return to the top-down view
            {
                selected = false;
                playerCamObj.SetActive(false);
                mainCam.SetActive(true);
                camScript.camLockState(false);
                buildingSystem.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    //Gets called when the mouse hovers over the collider of the GameObject the script is attached to
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !selected)//select character if we click while currently hovering over the character
        {
            selected = true;

            //calling the necessary locks
            camScript.camLockState(true);
            buildingSystem.SetActive(false);

            //save main camera info
            mainCamOriginalPos = mainCam.transform.position;
            mainCamOriginalRotation = mainCam.transform.rotation;
            
            //set target camera info
            targetPos = playerCamObj.transform.position;
            targetRot = playerCamObj.transform.rotation;
        }
    }

    void moveCamera(GameObject cam, Vector3 pos, Quaternion rot)//interpolates camera from it's original position and rotation
                                                                //to the selected character's camera position and rotation
    {
        float interRatio = Time.deltaTime * 3f;
        cam.transform.position = Vector3.Slerp(cam.transform.position, pos, interRatio);
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, rot, interRatio);
    }

    void movePlayer()
    {
        //deltas stored here
        Vector3 temp = Vector3.zero;
        Quaternion rotationQuat = Quaternion.AngleAxis(transform.rotation.y, Vector3.up);
        
        float speed = 8f;//speed of walking

        //move character with WASD keys
        if (Input.GetKey(KeyCode.W))
        {
            temp += transform.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            temp += transform.forward * -1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            temp += transform.right * -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            temp += transform.right;
        }

        //play appropiate animation
        if (temp != Vector3.zero) anim.SetBool("walking", true);
        else anim.SetBool("walking", false);

        //move the character
        transform.position += rotationQuat * temp * Time.deltaTime * speed;

        //move the camera
        LookAtMouse();
    }

    void LookAtMouse()//locks mouse in the middle of the screen and makes the camera look at it
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(mouseX * Vector3.up);
    }

}
