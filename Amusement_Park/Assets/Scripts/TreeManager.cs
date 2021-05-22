using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{   
    private float time;
    public float time_interval;
    public float radius;
   
    public int moodPoints;
    //Animator radiusAnim;

    // Start is called before the first frame update
    void Start()
    {   
        time = 0f;
        //radiusAnim = gameObject.transform.Find("Radius").GetComponent<Animator>();  /*For the expanding Dome once the tree refreshes its guests*/
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time = Mathf.Abs(time + Time.deltaTime);
        if(time >= time_interval){
            time -= time_interval;

            ///Increases the mood of the guests who still in its radius after a while.
            GameObject[] allguests = GameObject.FindGameObjectsWithTag("Guest");
            foreach(GameObject guest in allguests){
                if(inRange(guest.transform)){
                    Guest g_script = guest.GetComponent<Guest>();
                    g_script.subMood(-moodPoints);
                }
            }

            //radiusAnim.Play("visual_treat_tree");
        }
    }


    /*
    * Checks if a given transform is in the given radius.
    */
    private bool inRange(Transform guestT){
        float distanceSqr = (guestT.position - gameObject.transform.position).sqrMagnitude;     /// distance of 2 vectors in squares (faster than square rooting )
        return (distanceSqr <= radius * radius );
    }


    /*
    * When a Guest reaches the Area it increases its mood.
    */
    private void OnTriggerEnter(Collider go) {
        if(go.tag == "Guest"){
            Guest g_script = go.GetComponent<Guest>();
            g_script.subMood(-moodPoints);
        }
    }
}
