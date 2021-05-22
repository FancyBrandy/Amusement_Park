using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    // Start is called before the first frame update
    NavMeshSurface surf;


    /*
    * Bake the scene when it starts
    */
    void Awake()
    {
        surf = gameObject.GetComponent<NavMeshSurface>();
    }

    void Start(){
        surf.BuildNavMesh();
    }


    public void bakePaths(){
        surf.BuildNavMesh();
    }
}
