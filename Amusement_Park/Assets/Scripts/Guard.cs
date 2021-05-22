using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum GuardStates{
    Idle, Roaming, Chasing, Escorting
}

public class Guard : MonoBehaviour
{
    
    GuardStates status;
    NavMeshAgent navMeshAgent;
    GameObject chasedTheif; // the chased theif 
    Animator anim;

    
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        status = GuardStates.Roaming;
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

    }

    
    void FixedUpdate()
    {
        switch (status)
        {
            case GuardStates.Idle:
                chooseRandomPath();
            break;
            case GuardStates.Roaming:
                if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Guard_Walk")) anim.Play("Guard_Walk");
                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance){
                    status = GuardStates.Idle;
                    anim.CrossFadeInFixedTime("Guard_Idle", 0.1f);
                }
            break;
            case GuardStates.Chasing:
                if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Guard_Run")) anim.Play("Guard_Run");
                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance){
                    catchTheif();
                }else{
                    follow(this.chasedTheif);
                }
            break;
            case GuardStates.Escorting:
                if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Guard_Walk")) anim.Play("Guard_Walk");
                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance){
                    this.status = GuardStates.Idle;
                    this.chasedTheif = null;
                }else{ // checks if a path is valid to the given security building or change it.
                    catchTheif();
                }
            break;
            default:
            break;
        }
    }


    
    private void chooseRandomPath(){
        GameObject[] paths = GameObject.FindGameObjectsWithTag("Path_Block");
        if(paths.Length == 0){
            return;
        }

        int rand = Random.Range(0, paths.Length);
        NavMeshPath destination = new NavMeshPath();
        navMeshAgent.CalculatePath(paths[rand].transform.position, destination);
        if(destination.status == NavMeshPathStatus.PathComplete){
            navMeshAgent.speed = 1f;
            navMeshAgent.SetDestination(paths[rand].transform.position);
            status = GuardStates.Roaming;
        }
    }

    /*Follow the theif object*/
    public void follow(GameObject theif){
        this.chasedTheif = theif;
        if(theif){
            NavMeshPath destination = new NavMeshPath();
            navMeshAgent.CalculatePath(theif.transform.position, destination);
            if(destination.status == NavMeshPathStatus.PathComplete){
                navMeshAgent.speed = 5f;
                navMeshAgent.SetDestination(theif.transform.position);
                status = GuardStates.Chasing;

            }
        }else{
            status = GuardStates.Idle;
            this.chasedTheif = null;
        
        }  
    }

    /*checks if it reached the theif and do the catching logic .. change theif destination + change guard destination and states.*/
    private void catchTheif(){
        if(!chasedTheif){ /// if thief ran away...
            this.status = GuardStates.Idle;
            this.chasedTheif = null;
        }else{  /// catching thief
            GameObject securityBuilding = GameObject.Find("SecurityBuilding");
            if(!securityBuilding){
                return;
            }
            /// heading to polic department . Check if path exists.
            NavMeshPath destination = new NavMeshPath();
            if(!navMeshAgent.CalculatePath(securityBuilding.transform.Find("Entrance").position, destination))
                return;

            if(destination.status == NavMeshPathStatus.PathComplete){
                navMeshAgent.speed = 1f;
                navMeshAgent.SetDestination(securityBuilding.transform.Find("Entrance").position);
                chasedTheif.GetComponent<Thief>().goToPolice(securityBuilding); ///Ordering the thief to go with them
                status = GuardStates.Escorting;
            }else{
                Debug.Log("No path to the police station is found  =>  Setting free the theif.");
                chasedTheif.GetComponent<Thief>().setFree(); /// No path to the desired Security building. The theif is set free
                status = GuardStates.Idle;                  /// the police returns back to roaming around
            }
        }
        
    }
}
