using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * For dynamically placing UI buttons on specific gameObjects in the world
 */
public class ButtonsOverModels : MonoBehaviour
{ 
    public Transform target;//designated position of the buttons on the GameObject

    private RectTransform[] buttons;//collection of all the UI buttons of the GameObject this script is attached to
    private Image[] images;//collection of the images of the UI buttons

    // Awake is called when an instance of this script is loaded
    private void Awake()
    {
        //Initializing variables
        buttons = GetComponentsInChildren<RectTransform>(true);
        images = GetComponentsInChildren<Image>(true);
        
        //place the buttons in their desginated place on the GameObject
        MoveButtons();
    }

    // Called every frame
    private void Update()
    {
        if (Camera.main != null) MoveButtons();//if the camera is active, continously update the position of the button
        else HideButtons();//otherwise hide them since we don't need them
    }

    private void MoveButtons()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.position); //translate a point in the scene to a screen space (3d -> 2d)

        for (int i = 0; i < images.Length; i++)//show all the images
        {
            images[i].enabled = true;
        }

        for (int i = 1; i < buttons.Length; i++)//move them all to the screenpoint
        {
            buttons[i].position = screenPoint;
        }
        
    }

    private void HideButtons()
    {
        for (int i = 0; i < images.Length; i++)//hide all images
        {
            images[i].enabled = false;
        }
    }
}
