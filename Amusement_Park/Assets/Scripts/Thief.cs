using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum ThiefStates{
    Idle, Walking, Running, Escorted
}

public class Thief : MonoBehaviour
{

    NavMeshAgent navMeshAgent;  // navigator
    Vector3 spawnPosition;      // home (sewer)
    Animator anim;
    ThiefStates status;
    GameObject chosenGuest;
    int level;


    void Start()
    {
        level = Random.Range(0,101);        // 0 to 100
        this.status = ThiefStates.Idle;
        navMeshAgent = GetComponent<NavMeshAgent>();
        spawnPosition = gameObject.transform.position;
        anim = GetComponent<Animator>();
        //Debug.Log(level + " Level" );
    }



    void FixedUpdate()
    {   
        switch(status){
            case ThiefStates.Idle:
                if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) anim.Play("Idle");
                pickVictim();
            break;
            case ThiefStates.Walking:
                if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")) anim.Play("Walk");
                if(!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && chosenGuest.activeSelf){
                    interactGuest();
                }else if(!navMeshAgent.pathPending){
                    followGuest();
                }
            break;
            case ThiefStates.Running:
                if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Run")) anim.Play("Run");
                if(!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance){
                    escape();
                }else{ /// escape path still exists and not blocked
                    NavMeshPath path = new NavMeshPath();
                    if(!navMeshAgent.CalculatePath(spawnPosition, path)) return; /// No existance to destination.
                    if(path.status == NavMeshPathStatus.PathComplete){
                        navMeshAgent.SetDestination(spawnPosition);
                    }else{
                        setFree();      // transforming him back to idle => his normal activities till he got caught
                    }
                }
            break;
            case ThiefStates.Escorted:
                if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")) anim.Play("Walk");
                if(!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance){
                    GameObject.Destroy(gameObject);
                } /// checking path exists are done in the Guards
            break;

            default:
            break;
        }        
    }


    //Choosing a random guest to pick his pocket.
    void pickVictim(){
        
        GameObject[] guests = GameObject.FindGameObjectsWithTag("Guest");
        if(guests.Length == 0) return;

        int rand = Random.Range(0 , guests.Length);

        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(guests[rand].transform.position, path);
        if(path.status == NavMeshPathStatus.PathComplete){
            navMeshAgent.SetDestination(guests[rand].transform.position);
            chosenGuest = guests[rand];
            //Debug.Log("victim: " + rand);
            this.status = ThiefStates.Walking;
        }
    }


    /*Follows teh guest wherever he goes .. it also checks if road was cut between the thief and guest*/
    void followGuest(){
        if(!chosenGuest){ /// if guests left the park or just got destroyed
            this.status = ThiefStates.Idle; 
            //Debug.Log("Chosen Guest destroyed");
            return;
        }

        if(!chosenGuest.activeSelf) {// stop chasing if the guests enters a ride
            chosenGuest = null;
            this.status = ThiefStates.Idle; //Debug.Log("Chosen Guest not active");
            navMeshAgent.isStopped = true;
            return;
        }

        NavMeshPath path = new NavMeshPath();

        if(!navMeshAgent.CalculatePath(chosenGuest.transform.position, path))
            return;
            
        
        /*
        //Debug.Log("Calculation failed");  ///Takes some split of seconds to be calculated .. if the path was not calculated by next step an error will occur...
        switch(path.status){
            case NavMeshPathStatus.PathPartial:
                Debug.Log("Partiial path");
            break;
            case NavMeshPathStatus.PathInvalid:
                Debug.Log("Invalid path");
            break;
            case NavMeshPathStatus.PathComplete:
                Debug.Log("Valid path");
                break;
            default:
            break;
        }*/
        
        if(path.status == NavMeshPathStatus.PathComplete){
            navMeshAgent.SetDestination(chosenGuest.transform.position);
        }else{
            this.status = ThiefStates.Idle;
            chosenGuest = null;
            navMeshAgent.ResetPath();
        }
    }


    /*
    * Interact with the guest if it reached him... Thief/guest interaction.
    */
    void interactGuest(){
        int resistance = Random.Range(0,101);
        if(resistance < level){     /// theif won, he steals and continue his day...
            chosenGuest.GetComponent<Guest>().stealMoney(level - resistance);     /// subtracting money.
            chosenGuest = null;
            this.status = ThiefStates.Idle;
        }else{                      /// calling security
            chosenGuest.GetComponent<Guest>().subMood(70);       // impacting the guest mood by 5
            callAllPolice();                                     // calling all the police in the area toward the theif
        }
    }

    /*
    * Calling all police to the thief
    */
    void callAllPolice(){
        //caling the police
        GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard"); 
        foreach(GameObject grd in guards){
            grd.GetComponent<Guard>().follow(gameObject);
        }

        //running to the initialization spot
        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(spawnPosition, path);
        if(path.status == NavMeshPathStatus.PathComplete){
            navMeshAgent.SetDestination(spawnPosition);
            navMeshAgent.speed = 6f;
            chosenGuest = null;
            this.status = ThiefStates.Running;
            return;
        }
        chosenGuest = null;
        this.status = ThiefStates.Idle;
    }

    /*
    * Thief escaped
    */
    void escape(){
        GameObject.Destroy(gameObject);
    }

    /*
    * Called from the Guard.. An order to set the destination of the theif to the given security building
    */
    public void goToPolice(GameObject securityBuilding){
        this.status = ThiefStates.Escorted;
         navMeshAgent.speed = 1f;
         navMeshAgent.SetDestination(securityBuilding.transform.Find("Entrance").position);
    }

    /*
    * Called if a path not found to the security building.. the escorting team set the theif back to freedom
    */
    public void setFree(){
        this.status = ThiefStates.Idle;
        navMeshAgent.speed = 3.5f;
    }



     /*
    * Get the distance between the starting point of a path and the ending point
    */
    private float getPathLength( NavMeshPath path )
    {
        float lng = 0f;

        if ( path.status == NavMeshPathStatus.PathComplete)
        {
            for ( int i = 0; i < path.corners.Length; i++ ) // Summation algorithm
            {
                if( i == 0 ){
                    lng = Vector3.Distance(gameObject.transform.position, path.corners[0]);
                }else{
                     lng += Vector3.Distance( path.corners[i - 1], path.corners[i] );
                }
            }
        }
        return lng;
    }
}
