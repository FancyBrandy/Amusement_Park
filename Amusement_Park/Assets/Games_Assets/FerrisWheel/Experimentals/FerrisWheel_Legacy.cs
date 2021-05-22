using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FerrisWheelLegacy{
    public enum GameStates {
        Running, Idle, Constructing
    }

    public class FerrisWheel : MonoBehaviour
    {


        public bool running; 
        private Animation anim;
        // Start is called before the first frame update
        void Start()
        { 
            anim = GetComponent<Animation>();
            //anim.Blend("Stopped");
            anim.Blend("Idle");  
        }

        // Update is called once per frame
        void Update()
        {


            //Debug.Log(anim.GetClipCount());
            foreach (AnimationState state in anim)
            {
                Debug.Log(state.name);
            }


            if(running){
            anim.Blend("Running");  
            }else{
                //anim.CrossFade("Running");
                anim.Stop("Running");  
            }
            
        }
    }
}

