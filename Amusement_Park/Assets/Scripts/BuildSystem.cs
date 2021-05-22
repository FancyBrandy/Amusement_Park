 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.AI; ///for rebaking the scene everytime a new object is placed.


/**
 * Script for handling all the building logic
 */
public class BuildSystem : MonoBehaviour
{
    public Camera cam;//camera used for raycast
    public LayerMask buildLayer;//the layer that building raycast will hit on
    public LayerMask destroyLayer;//the layer that the destroying raycast will hit on
    public LayerMask cleanerLayer;//the layer that selects cleaners
   
    private GameObject previewGameObject = null;//referance to the preview gameobject
    private GameObject originalGameObject = null;//previewGameObject will have a clone, this one will have the original
    private Preview previewScript = null;//the Preview.cs script sitting on the previewGameObject

    private GameObject finalGameObject = null;//referance to the final gameobject
    private GameObject selectedCleaner = null;//referance to the selected cleaner

    [HideInInspector] //hiding this in inspector, so it doesnt accidently get clicked
    public bool isBuilding = false;//used to pause building mode
    [HideInInspector]
    public bool isDestroying = false;//used to enter destroy mode
    private bool pauseBuilding = false;//used to pause the raycast
    private Vector3 savedMousePos = Vector3.zero;//used to calculate mouse deviation
    
    //constraints
    private float minDistance = 0f;
    private float maxDistance = 300f;
    private bool turned = false;
    public DynamicUI dynamicUI;
    public Game_Specific_Script gs;

    private void Update()
    {
        if (previewGameObject && Input.GetKeyDown(KeyCode.R))//rotate
        {
            previewGameObject.transform.Rotate(0, 90f, 0);//rotate the preview 90 degrees.
            turned = !turned;
        }

        if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(1))//cancel build
        {
            CancelBuild();
        }

        if (Input.GetMouseButtonDown(0) && isBuilding && previewScript.isReadyToBuild())//actually build the thing in the world
        {
            ConfirmBuild();
        }

        if (Input.GetMouseButtonDown(0) && isDestroying)//remove gameobject from the world
        {
            if (finalGameObject)
            {
                RideManager rideManagerScript = finalGameObject.GetComponent<RideManager>();//we need the preDestroy() function of this script
                if (rideManagerScript) rideManagerScript.preDestroy();
                DestroyBuild();
            }
        }

        if (isBuilding)
        {   
            if (pauseBuilding)//is the build system currently paused? if so then we need to check deviation in the mouse 
            {
                Vector3 delta = Input.mousePosition - savedMousePos;//get the mouse deviation
                float stickTolerance = 2000f / cam.transform.position.y;

                if (Mathf.Abs(delta.x) >= stickTolerance || Mathf.Abs(delta.y) >= stickTolerance)//check if horizontal or vertical value is greater than stickTolerance
                {
                    pauseBuilding = false;//if it is, then unpause building, and call the raycast again
                }

            }
            else//if building system isn't paused then call the raycast
            {
                DoBuildRay();
            }
        }

        if (isDestroying)
        {
            DoDestroyRay();
        }

        if(!isBuilding && !isDestroying)
        {
            DoCleanerRay();
        }

        if (selectedCleaner)
        {
            //make the cleaner look toward the camera
            Vector3 lookAt = cam.transform.position - selectedCleaner.transform.position;
            lookAt.y = 0f;
            selectedCleaner.transform.rotation = Quaternion.Slerp(selectedCleaner.transform.rotation, Quaternion.LookRotation(lookAt), Time.deltaTime * 5); ;
        }
    }

    public void NewBuild(GameObject _go) //starts the building process
    {
        if (isBuilding) CancelBuild();
        originalGameObject = _go;
        previewGameObject = Instantiate(_go, _go.transform.position, _go.transform.rotation);
        previewScript = previewGameObject.GetComponent<Preview>();
        isBuilding = true;
        isDestroying = false;
        turned = false;
       
       
    }

    private void CancelBuild()//this will get rid of the previewGameObject in the scene
    {
        Destroy(previewGameObject);
        originalGameObject = null;
        previewGameObject = null;
        previewScript = null;
        isBuilding = false;
        turned = false;
        
    }

    private void ConfirmBuild()//build the preview in the world
    {
      
        previewScript.Place();
       
       
        //UnityEditor.AI.NavMeshBuilder.BuildNavMesh(); //By Mohido: Rebaking the scene after a building is confirmed an added. Hope I placed in the right place.
       
        if (previewScript.isPath)
        {
            NewBuild(originalGameObject);
    
        }
        else
        {
            originalGameObject = null;
            previewGameObject = null;
            previewScript = null;
            isBuilding = false;
        }
    }

    public void DestroyBuild()
    {
        gs = finalGameObject.GetComponent<Game_Specific_Script>();
        int seats = (int)(gs.getGameSeatsCount() * 1.5f); 
        dynamicUI.changeGuestLimit(-seats); // subtracting the guests limit since it depends on the ride
        dynamicUI.changeMoney(gs.getGameMoney()/2);

        /*Bake paths when building a building or a pathblock*/
        if (finalGameObject.CompareTag("Path_Block") || finalGameObject.CompareTag("Building")){
            DestroyImmediate(finalGameObject);
            GameObject.Find("PathContainer").GetComponent<NavMeshBaker>().bakePaths();
        }
        else{
            Destroy(finalGameObject);
        }
       
    }

    public void PauseBuild(bool _value)//public method to change the pauseBuilding bool from another script
    {
        savedMousePos = Input.mousePosition;
        pauseBuilding = _value;
    }

    private void DoBuildRay()//positions your previewGameobject in the world
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);//raycast
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, buildLayer))
        {
            //constraints of the posotion of the preview GameObject in the scene
            Vector3 center = previewGameObject.transform.GetComponent<BoxCollider>().size;
            Vector3 scale = previewGameObject.transform.localScale;
            float xBound = center.x / 2f * scale.x;
            float zBound = center.z / 2f * scale.z;

            if (turned)//if we rotated the preview GameObject, then switch the bounds
            {
                float temp = xBound;
                xBound = zBound;
                zBound = temp;
            }

            float x = Mathf.Clamp(hit.point.x, minDistance + xBound, maxDistance - xBound);
            float z = Mathf.Clamp(hit.point.z, minDistance + zBound, maxDistance - zBound);
            float y = hit.point.y + (scale.y / 2f);

            Vector3 pos = new Vector3(x, y, z);
            previewGameObject.transform.position = pos;

            //raycast for other pre-built 3d models with correct anchor points 
            //previewGameObject.transform.position = hit.point;
        }
    }

    private void DoDestroyRay()//Choose which gameobject to destroy
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);//raycast
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, destroyLayer))
        {
            finalGameObject = hit.transform.gameObject;
        }
        else finalGameObject = null;
    }

    private void DoCleanerRay()
    {
        if (!selectedCleaner)//if we dont have a cleaner selected choose a cleaner
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);//raycast
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, cleanerLayer))
            {
                GameObject cleaner = hit.transform.gameObject;
                if (Input.GetMouseButtonDown(0))
                {
                    selectedCleaner = cleaner;
                    selectedCleaner.transform.Find("Ring_Final").gameObject.SetActive(true);//activate visual ring
                }
            }
        }
        else //if we have a selected cleaner, we just record the raycast position of the next click
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);//raycast
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0))
            {
                LayerMask pathLayer = LayerMask.GetMask("PathLayer");
                if (Physics.Raycast(ray, out hit, float.PositiveInfinity, pathLayer)) //if clicked on a path
                {
                    selectedCleaner.GetComponent<Cleaner>().navigateTo(hit.point);
                }
                else
                {
                    selectedCleaner.GetComponent<Cleaner>().navigateTo(selectedCleaner.transform.position);
                    selectedCleaner.transform.Find("Ring_Final").gameObject.SetActive(false);//deactivate visual ring
                }
                selectedCleaner = null;
            }
        }
    }

    public void ToggleDestroyMode()
    {
        if(!isBuilding) isDestroying = !isDestroying;
    }
}
