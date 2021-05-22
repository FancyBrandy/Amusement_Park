using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;


public class Garden : MonoBehaviour
{

    Queue<GameObject>     currentGuests;
    ArrayList             plants;
 

    void Start()
    {   
        plants=new ArrayList();
        GameObject[] people = GameObject.FindGameObjectsWithTag("Guest");// find all the game objects with the tag guest
        foreach(GameObject p in people)
        {
            currentGuests.Enqueue(p);
        }
            foreach(GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
            {
                if(go.name == "BTree_A"||go.name == "Sakura_A"||go.name == "Maple_A")
                    plants.Add (go);
            }   // find all the plants existing
        while(Application.isPlaying)
        {
            updateMood();
        }
    }

    Transform GetClosestEnemy(ArrayList nearby, GameObject targetGuest)
        {
            Transform tMin = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = targetGuest.transform.position;
            foreach (GameObject t in nearby)
            {
                float dist = Vector3.Distance(t.transform.position, currentPos);
                if (dist < minDist)
                {
                    tMin = t.transform;
                    minDist = dist;
                }
            }
            return tMin;
        }

    IEnumerator ExampleCoroutine()
     {
         //yield on a new YieldInstruction that waits for 5 seconds.
         yield return new WaitForSeconds(5);
     }

    void updateMood(){
                 foreach (GameObject guest in currentGuests)   /// setting all players' mood
                    {
                        Vector3 v=guest.transform.position-GetClosestEnemy(plants,guest).position;
                        if(v.magnitude>100.0)
                        {
                            diminishMood(5);
                        }
                        else
                        {
                            incrementMood(5);
                        }
                    }
         StartCoroutine(ExampleCoroutine());//delay for 5 seconds
         
    }


     private void diminishMood(int mood){
            int queueCount = currentGuests.Count;   /// number of people to diminish.
            for( int i = 0 ; i < queueCount ; i++){
                GameObject guest = currentGuests.Dequeue();
                Guest g_script = guest.GetComponent<Guest>();// dequeue the player to change their information
                if(g_script){
                    g_script.subMood(mood);    // subtracts from the mood of the guest
                    if(g_script.isDestroyed() == false){
                        currentGuests.Enqueue(guest);   // if the guest is not destroyed yet. then add it to the end of the same queue (End will be begning after all iterations.)
                    }
                }
            }
     }
       private void incrementMood(int mood){
            int queueCount = currentGuests.Count;   /// number of people to diminish.
            for( int i = 0 ; i < queueCount ; i++){
                GameObject guest = currentGuests.Dequeue();
                Guest g_script = guest.GetComponent<Guest>();// dequeue the player to change their information
                if(g_script){
                    g_script.subMood(-mood);    // subtracts 10 from the mood of the guest
                    if(g_script.isDestroyed() == false){
                        currentGuests.Enqueue(guest);   // if the guest is not destroyed yet. then add it to the end of the same queue (End will be begning after all iterations.)
                        Debug.Log("one of the guest leavs the park");
                    }
                }
            }
     }
            
}
