using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


enum CleanerStates {
    Walking, Idle // Idle is cleaning.
}

public class Cleaner : MonoBehaviour
{

    NavMeshAgent    navMeshAgent;                 /// Navigation component.
    Vector3         spot;
    float           radius;
    CleanerStates   status;
    GameObject      dirtInMind;
    Vector3         destination;
    NavMeshPath     path;
    GameObject      halo;
    
    /*Animating properties*/
    Animator anim;
    public Material mat;

    object lock_;

    GameObject[] dirts;
    void Start()
    {
        lock_ = new object();
        navMeshAgent = GetComponent<NavMeshAgent>();
        status = CleanerStates.Idle;
        anim = gameObject.GetComponent<Animator>();
        dirtInMind = null;
        radius = 10f;
        spot = gameObject.transform.position; //Supposed to be the spawn position (Entrance)
    }

    void FixedUpdate()
    {
        switch(status){
            case CleanerStates.Idle:
                dirts = GameObject.FindGameObjectsWithTag("Garbage");
                anim.SetBool("isWalking", false);
                cleanArea();            // clean the surrounding area
            break;
            case CleanerStates.Walking:
                anim.SetBool("isWalking", true);

                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance){
                    lock(lock_){
                        //Debug.Log(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance);
                        interactPosition();     // checks if reached a garbage or its new destination.
                    }
                }
            break;
            default:

            break;
        }
    }

    void Update()
    {
        if(status == CleanerStates.Walking && path != null)
        {
            navMeshAgent.CalculatePath(destination, path);
            drawLinePath(path);
        }
    }

    void cleanArea(){
        
        if(dirts.Length == 0)
            return;

        /*select a garbage in the cleaning area*/
        
        bool found = false;
        for(int i = 0 ; i < dirts.Length && !found; i++){
            if(inRange(dirts[i].transform)){// if in range
                NavMeshPath path = new NavMeshPath();
                navMeshAgent.CalculatePath(dirts[i].transform.position, path);
                if(path.status == NavMeshPathStatus.PathComplete){ // if there is a path to it.
                    navMeshAgent.SetDestination(dirts[i].transform.position);
                    this.dirtInMind = dirts[i];
                    found = true;
                    this.status = CleanerStates.Walking;
                    anim.SetBool("isWalking", true);
                    //anim.CrossFadeInFixedTime("Zombie_Crawl",0.5f);
                    return;
                }
            }
        }
    }


    /*
    * 
    */

    void interactPosition(){
        this.status = CleanerStates.Idle;
        anim.SetBool("isWalking", false);
        if (dirtInMind) /// if the interaction was with the dirt.
        {
            GameObject.Destroy(dirtInMind);
        }
        else
        {
            Destroy(halo);
            transform.Find("Ring_Final").gameObject.SetActive(false);
        }
    }


    /*
    * A dirt is in the area.
    */
    private bool inRange(Transform dirtT){
        float distanceSqr = (dirtT.position - spot).sqrMagnitude;     /// distance of 2 vectors in squares (faster than square rooting )
        return (distanceSqr <= radius * radius );
    }

    /*
    * Navigate the Cleaner to a new position
    */
    public bool navigateTo(Vector3 position)
    {
        lock (lock_)
        {
            if (halo) Destroy(halo);
            destination = position;
            path = new NavMeshPath();
            navMeshAgent.CalculatePath(position, path); //calculate first side.

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                halo = transform.Find("Ring_Final").gameObject;
                halo = Instantiate(halo, position, halo.transform.rotation);
                drawLinePath(path);
                navMeshAgent.SetDestination(position);
                dirtInMind = null;
                spot = position;
                status = CleanerStates.Walking;
                //anim.CrossFadeInFixedTime("Zombie_Crawl",0.5f);
                anim.SetBool("isWalking", true);

            }
            else
            {
                return false;
            }
            return true;
        }

    }


    void drawLinePath(NavMeshPath path) 
    {
        for ( int i = 0; i < path.corners.Length; i++ ) // Summation algorithm
            {
            if( i == 0 ){
                GameObject myLine = new GameObject();
                myLine.AddComponent<LineRenderer>();
                LineRenderer lr = myLine.GetComponent<LineRenderer>();
                lr.material = mat;
                lr.startColor = Color.black;
                lr.endColor = Color.black;

                Vector3 start = gameObject.transform.position;
                Vector3 end = path.corners[0];

                myLine.transform.position = start;

                lr.startWidth = 0.1f;
                lr.endWidth =  0.1f;
                lr.SetPosition(0, start);
                lr.SetPosition(1, end);

                GameObject.Destroy(myLine, 0.1f);

                //Gizmos.DrawLine(gameObject.transform.position, path.corners[0]); //Vector3.Distance(gameObject.transform.position, path.corners[0]);
            }else{
                GameObject myLine = new GameObject();
                myLine.AddComponent<LineRenderer>();
                LineRenderer lr = myLine.GetComponent<LineRenderer>();
                lr.material = mat;
                lr.startColor = Color.black;
                lr.endColor = Color.black;

                Vector3 start = path.corners[i - 1];
                Vector3 end = path.corners[i];
                
                myLine.transform.position = start;

                lr.startWidth= 0.1f;
                lr.endWidth =  0.1f;
                lr.SetPosition(0, start);
                lr.SetPosition(1, end);

                GameObject.Destroy(myLine, 0.1f);

                //Gizmos.DrawLine(path.corners[i - 1], path.corners[i]);
                 //lng += Vector3.Distance( path.corners[i - 1], path.corners[i] );
            }
        }

    }

}