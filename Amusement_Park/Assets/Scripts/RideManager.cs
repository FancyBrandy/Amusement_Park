using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates {
        RUNNING, IDLE , BUILDING, BROKEN, REPAIRING
}

public class RideManager : MonoBehaviour
{
    
    //_______________________________________________________
    GameStates            status = GameStates.IDLE;        

    float                 runningTime_ms;
    float                 stoppingTime;             /// Running animation time

    float                 stopBuildingTime;         /// The time of the build
    float                 buildingtime;             /// The current time since building started


    GameObject            repairman;
    float                 stoprepairingTime;
    float                 repairingTime;


    ArrayList             currentPlayers;           /// players that are playing
    Queue<GameObject>     queuedPlayers;            /// players waiting their turn
    object                lock_;                    /// avoid chaos when multiple players call the addplayer() function!  
    Animator              m_animator;               /// changing the ride animation
    Game_Specific_Script  gameScript;               /// main script for rendering and getting specific building details
    
    public DynamicUI dUIScript;
    int seatCount;
    //_______________________________________________________
    /// static for now.. For testing purposes!
     /// we need 11 players for running the game.

    void Start()
    {   
        currentPlayers = new ArrayList();
        queuedPlayers = new Queue<GameObject>();
        lock_ = new object(); /// a new object that we will lock upon..  every game has its unique game object.
        gameScript = GetComponent<Game_Specific_Script>();
        seatCount = gameScript.getGameSeatsCount();
        m_animator = gameScript.getGameAnimator();
        
        /*Running states*/
        runningTime_ms = 0.0f;
        
        foreach (AnimationClip clip in m_animator.runtimeAnimatorController.animationClips) //For all animations
        {
            if (clip.name == "Running")        //If it has the desired clip
            {
                stoppingTime = clip.length / m_animator.GetFloat("speed"); // get the length of the animation in seconds
            }
        }


        /*Activate Building Animation and time.*/
        this.status = GameStates.BUILDING;
        stopBuildingTime = gameScript.getGameBuildingTime();
        buildingtime = 0.0f;
        gameScript.activate_smoke();
        gameScript.activate_buildingAnime();
        
          
        /*Reparing states has to be set after the user clicks on repair and startRepairing() happens*/
        stoprepairingTime = (float)Random.Range(60, 180);     // 1 min to 3 mins: (6 mins is quarter of a day in the game)
        repairingTime = 0.0f;

        // Increasing the park guests limit
        int seats = gameScript.getGameSeatsCount();
        GameObject.Find("DynamicUI").GetComponent<DynamicUI>().changeGuestLimit( (int) (seats * 1.5f) );  // change the guest account in UI

        
    }


    void FixedUpdate()
    {
        
        switch (status)
        {
            case GameStates.BUILDING:
                buildingtime += Time.fixedDeltaTime;
                if(buildingtime > stopBuildingTime){
                    /*Deactivate animations*/
                    buildingtime = 0.0f;
                    gameScript.deactivate_smoke();
                    gameScript.deactivate_buttons();

                   
                    status = GameStates.IDLE;
                }
            break;

            case GameStates.IDLE:
                m_animator.SetBool("isRunning", false);  
                runningTime_ms = 0.0f; 
                if(currentPlayers.Count == seatCount) /// game ready to start...
                    startGame();
            break;

            case GameStates.RUNNING:
                m_animator.SetBool("isRunning", true);
                runningTime_ms += Time.fixedDeltaTime;
                if(runningTime_ms >= stoppingTime){ /// reached maximum running time
                    runningTime_ms = 0.0f;
                    gameEnd();
                    diminishMood();
                }else{
                    int breakingChance = Random.Range(0,30000); /// Huge chance it will not breakup
                    if(breakingChance == 0){
                        preDestroy();
                        breakUp();
                    }
                }

            break;

            case GameStates.BROKEN:
                m_animator.SetBool("isRunning", false);
                gameScript.activate_smoke();
                gameScript.activate_repairButtons();
            break;

            case GameStates.REPAIRING:
                m_animator.SetBool("isRunning", false);
                repairingTime += Time.fixedDeltaTime;
                if(repairingTime > stoprepairingTime){
                    repairingTime = 0.0f;
                    gameScript.deactivate_smoke();
                    gameScript.deactivate_buttons();
                    this.repairman.GetComponent<Repairer>().finishRepairing();
                    repairman = null;
                    status = GameStates.IDLE;
                }
                
            break;


            default:
                break;
        }
    }




    /**
    *   Adds player to the game or queue if game is full!
    *   This function will be called from the Guest game object.
    */
    public void addPlayer(GameObject guest){
        if(status == GameStates.IDLE || status == GameStates.RUNNING){
            lock (lock_) /// lock the current ride
            {
                    if(currentPlayers.Count < seatCount && this.status == GameStates.IDLE){   /// if there is space.
                        currentPlayers.Add(guest);  
                        guest.SetActive(false);
                    }else{ /// if no space, queue them.
                        queuedPlayers.Enqueue(guest); 
                        guest.SetActive(false);
                    }
            
            }  
        }else{
            guest.GetComponent<Guest>().shiftToIdle();
        }
    }

    /*
    * Basically .. Clears the whole Game from players
    */
    public void preDestroy(){
        lock(lock_){
            status = GameStates.BROKEN;
            foreach (GameObject player in currentPlayers)   /// setting all players back to active.
            {
                player.GetComponent<Guest>().DonePlaying();
            }
            currentPlayers.Clear(); // no players on the game...
            int max_iter = queuedPlayers.Count;
            for ( int i = 0 ; i < max_iter ; i++){ // From queued to playing players
                queuedPlayers.Dequeue().GetComponent<Guest>().kickQueuer();
            }
        }
    }

    /**
    * Free current players from the machine
    * Move queued players to the machine's current players.
    */
    private void gameEnd(){
        lock(lock_){
            status = GameStates.IDLE;
            foreach (GameObject player in currentPlayers)   /// setting all players back to active.
            {
                player.GetComponent<Guest>().DonePlaying();
            }
            currentPlayers.Clear(); // no players on the game...
        }
        int max_iter = queuedPlayers.Count;
        for ( int i = 0 ; i < max_iter ; i++){ // From queued to playing players
            this.addPlayer(queuedPlayers.Dequeue());
        }
    }

    private void startGame(){
        this.status = GameStates.RUNNING; 
    }

    

    /*Call repairman*/
    public void callRepairman(){
        DynamicUI du = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        if(du.getMoney() >= gameScript.getRepairMoney()){ // check if the player has enough money for repairing
            GameObject[] repairmen = GameObject.FindGameObjectsWithTag("Repairman");
            if(repairmen.Length == 0)
                return;
            
            int j = 0;
            while(j < repairmen.Length && ! (repairmen[j].activeSelf && repairmen[j].GetComponent<Repairer>().isFree())){
                j++;
            }

            if(j < repairmen.Length){
                this.repairman = repairmen[j];
                repairman.GetComponent<Repairer>().call(gameObject);
            }

            BuildManager bm = GameObject.Find("BuildingSystem").GetComponent<BuildManager>();
            StartCoroutine(bm.toggleErrorPanel("Called a repairman!"));
        }else{ // Show error message that the user don't have enought money
            BuildManager bm = GameObject.Find("BuildingSystem").GetComponent<BuildManager>();
            StartCoroutine(bm.toggleErrorPanel("Not Enough Money!"));
        }
        

    }

    /*Called from the repair man...*/
    public void startRepair(){
       
        //subtracting repairing money 
        int gamePrice = gameScript.getRepairMoney();
        GameObject.Find("DynamicUI").GetComponent<DynamicUI>().changeMoney(-gamePrice);

        repairingTime = 0.0f;
        stoprepairingTime = (float) Random.Range(60,180); /// 1 min to 3 mins 
        status = GameStates.REPAIRING;
        gameScript.activate_smoke();
        gameScript.activate_repairAnim();
    }


    private void breakUp(){
        status = GameStates.BROKEN;
        gameScript.activate_smoke();
        gameScript.activate_repairButtons();
    }


    /*
    *   A helper function that is called after each game ends.
    *   Sees all the rest of the queue and diminish their moods.
    */
    private void diminishMood(){
        lock(lock_){
            int queueCount = queuedPlayers.Count;   /// number of people to diminish.
            for( int i = 0 ; i < queueCount ; i++){
                GameObject guest = queuedPlayers.Dequeue();
                Guest g_script = guest.GetComponent<Guest>();
                if(g_script){
                    g_script.subMood(50);    // subtracts 10 from the mood of the guest
                    if(g_script.isDestroyed() == false){
                        queuedPlayers.Enqueue(guest);   // if the guest is not destroyed yet. then add it to the end of the same queue (End will be begning after all iterations.)
                    }else{
                        g_script.leaveQueue();
                    }
                }
            }
        }
    }


    public GameStates getState(){
        return this.status;
    }
}

