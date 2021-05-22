using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Script for simulating a preview before we actually build something in the scene
 */
public class Preview : MonoBehaviour
{
    public GameObject prefab;//the prefab that represents this preview

    public Material goodMat;//green material
    public Material badMat;//red material
    public Material uglyMat;//yellow material

    private BuildSystem buildSystem;
    private bool isSnapped = false;
    private bool isColliding = false;
    [HideInInspector]
    public bool isPath = false;

    private List<Collider> colliders = new List<Collider>();//dynamic list of all of the colliders the object is colliding with in real time

    private void Start()
    {
        buildSystem = GameObject.FindObjectOfType<BuildSystem>();
        isPath = this.gameObject.name == "Path_Preview(Clone)";//check if the current object we are trying to build is a path
        ChangeColor();
    }

    public void Place()//logic for actually placing a building in the scene, used by BuildSystem.cs
    {
        if(prefab.CompareTag("Path_Block") ){   // getting the path container and initialize the prefab in it
            GameObject container = GameObject.Find("PathContainer");
            Instantiate(prefab, transform.position, transform.rotation, container.transform);
            container.GetComponent<NavMeshBaker>().bakePaths();
        }
        else if(prefab.CompareTag("Building")) { // get the path container for the rebaking process.
            Instantiate(prefab, transform.position, transform.rotation); // instantiating of the object
            GameObject container = GameObject.Find("PathContainer");
            container.GetComponent<NavMeshBaker>().bakePaths();
        }
        else Instantiate(prefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }



    private void ChangeColor()
    {

        if (!isColliding && (!isPath || (isPath && isSnapped))) //check if it is not colliding and if it is a path, we check if it is snapped
        
        {
            foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())//loop through all the renderers in itself and children
            {
                Material[] m = r.materials;//list all the materials
                for (int i = 0; i < m.Length; i++)//change them all
                {
                    m[i] = goodMat;
                }
                r.materials = m;//replace the materials of the object
            }
            Transform ent = gameObject.transform.Find("Entrance");
            if (ent) { ent.GetComponent<MeshRenderer>().material = uglyMat; }
        }
        else
        {
            foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())//loop through all the renderers in itself and children
            {
                Material[] m = r.materials;//list all the materials
                for (int i = 0; i < m.Length; i++)//change them all
                {
                    m[i] = badMat;
                }
                r.materials = m;//replace the materials of the object
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Final>()) //if we collided with any final buildings
        {
            colliders.Add(other);//we add it to the list of colliders we are currently colliding with
            isColliding = true;
            ChangeColor();
        }
    
        if(isPath && other.CompareTag("Path_SP"))//this is what dertermines if we are snapped to a snap point
        {
            buildSystem.PauseBuild(true); //since we are using a raycast to position the preview
                                          //when we snap to something we need to "pause" the raycast
            isSnapped = true;
            ChangeColor();

            transform.position = other.transform.position;//set position of path so that it "snaps" into position
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Final>())//check if we had collided with a final building
        {
            colliders.Remove(other);//remove it from the list
            if(colliders.Count == 0)//if we are currently colliding with 0 objects
            {
                isColliding = false;
                ChangeColor();
            }
        }

        if (isPath && other.CompareTag("Path_SP"))//this is what determines if the path is no longer snapped to a snap point
        {
            isSnapped = false;
            ChangeColor();
        }
        
    }

    public bool isReadyToBuild()//if not colliding and, if its a path, if it is snapped 
    {
        return (!isColliding && (!isPath || (isPath && isSnapped)));
    }

}
