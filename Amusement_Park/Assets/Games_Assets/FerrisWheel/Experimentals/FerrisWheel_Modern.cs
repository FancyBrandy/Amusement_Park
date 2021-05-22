using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FerrisWheel
{
    public enum GameStates {
      RUNNING, IDLE , BUILDING
    }   

    public class FerrisWheel_Modern : MonoBehaviour
    {
        float runningTime_ms = 0.0f;
        public GameStates state = GameStates.IDLE;
        Animator m_animator;
        // Start is called before the first frame update
        void Start()
        {
            m_animator = GetComponent<Animator>();
            
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            runningTime_ms += Time.deltaTime;


            if(state == GameStates.RUNNING){
                m_animator.SetBool("isRunning", true);   
            }else if(state == GameStates.IDLE){
                m_animator.SetBool("isRunning", false);   
            }
            
            
            if(runningTime_ms >= 30.0f){ /// stop the game after 3 seconods.
                runningTime_ms -= 3.0f;
                m_animator.SetBool("isRunning", false); 
                state = GameStates.IDLE;
            }
            

        }
    }

}
