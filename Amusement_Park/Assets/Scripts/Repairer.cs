using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum RepairmanStates{
    Idle, Walking
}

public class Repairer : MonoBehaviour
{

    NavMeshAgent navMeshAgent;
    Animator anim;
    RepairmanStates status;
    GameObject ride;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        status = RepairmanStates.Idle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch(status){
            case RepairmanStates.Idle:
                wonderAround();
            break;
            case RepairmanStates.Walking:
                if(!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance){
                    if(ride){
                        RideManager rm = ride.GetComponent<RideManager>();
                        rm.startRepair();
                        this.status = RepairmanStates.Idle;
                        gameObject.SetActive(false);
                    }else{
                        this.status = RepairmanStates.Idle;
                        anim.CrossFadeInFixedTime("Idle", 0.3f);
                    }
                }else if(ride) {
                    call(ride);
                }else{ // if he was wondering around
                    NavMeshPath tempPath = new NavMeshPath();
                    if(!navMeshAgent.CalculatePath(navMeshAgent.destination, tempPath)) return;
                    if(NavMeshPathStatus.PathPartial == tempPath.status){
                        this.status = RepairmanStates.Idle;
                        anim.CrossFadeInFixedTime("Idle", 0.3f);
                        navMeshAgent.ResetPath();
                    }else 
                        navMeshAgent.SetDestination(navMeshAgent.destination);
                    
                }
            break;
            default:
            break;
        }
    }

    void wonderAround(){             
        if(!navMeshAgent || !gameObject.activeSelf || ride != null) return;        
        
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Path_Block");           //get all buildings
        if(buildings.Length == 0){
            return;
        }

        int index = Random.Range(0, buildings.Length);                                    ///random building index.
        GameObject building = buildings[index];       
                       
        NavMeshPath path = new NavMeshPath();          
        navMeshAgent.CalculatePath(building.transform.position, path);     

        if (path.status == NavMeshPathStatus.PathComplete )
        {
            ///path found 
            navMeshAgent.SetDestination(building.transform.position);
            if( (navMeshAgent.destination - gameObject.transform.position).sqrMagnitude <= 1f ) return;
            status = RepairmanStates.Walking;
            anim.CrossFadeInFixedTime("Walk", 0.3f);
        }
    }


    public void call(GameObject ride){
        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(ride.transform.Find("Entrance").position, path);

        if(path.status == NavMeshPathStatus.PathComplete){
            this.ride = ride;
            navMeshAgent.SetDestination(ride.transform.Find("Entrance").position);
            status = RepairmanStates.Walking;
            anim.CrossFadeInFixedTime("Walk", 0.3f);
        }
    }

    public void finishRepairing(){
        gameObject.SetActive(true);
        this.ride = null;
        this.status = RepairmanStates.Idle;
        anim.CrossFadeInFixedTime("Idle", 0.3f);
    }

    public bool isFree(){
        return (ride == null);
    }
}
