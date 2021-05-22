using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


///Guest state
public enum States {
     Walking, Idle
}

public enum Model { ///These are the types of GUESTS , 2 Males models, 2 female models.
    TIMUR, MOHIDO, MIA, FANCY, RnadomMale, RandomFemale
}

public class Guest : MonoBehaviour
{   
    //_______________________________________________________
    public Model    sprite;                       /// determines which sprite should be assigned to the prefab.
    States          status;                       /// Sets the state of the guest.
    GameObject      building;                     /// The current building the guest has in mind.
    NavMeshAgent    navMeshAgent;                 /// Navigation component.
    bool is_destroyed;
    
    
    /*Garbage stuff*/
    private int garbage_chance = 10;             /// throwing garbage chance in each update
    private int droppingTime = 10;//s             /// Time to do dropping chance, change this to increase the dropping intervals gap
    private float time;                           /// total time of the game
    private float trashcan_range = 20f;
    Queue<GameObject> dirts;                      /// Dirts that will be dropped every interval if there is any dirt. (Gotten from restaurant.)

    /*Guest feelings*/
    int money; bool needsMoney;
    int hunger;
    int fullStomach=10;
    int mood; int maxMood;

    GameObject mood_object;
    /*Animating properties*/
    Animator anim;

    int checkingPathTime = 5; // check if path exists every 5 seconds
    float checkingCounter;
    

    //_______________________________________________________


    void Start()
    { 
        /*Rest of teh initialization of the Guest*/
        checkingCounter = 0f;
        is_destroyed = false;
        hunger=fullStomach; 
        navMeshAgent = GetComponent<NavMeshAgent>();
        status = States.Idle;
        dirts = new Queue<GameObject>();
        maxMood = 100;
        mood = maxMood;
        time = 0f;
        anim = gameObject.GetComponent<Animator>();
        mood_object = gameObject.transform.Find("Mood").gameObject;
        needsMoney = false;

        /* Entry money */
        int entryfee = GameObject.Find("DynamicUI").GetComponent<DynamicUI>().GetEntranceFee();
        if(entryfee > 1000) {
            this.mood = 0;
            leavePark();
        }else{
            money = 1000 - entryfee;                // initial money price
            GameObject.Find("DynamicUI").GetComponent<DynamicUI>().changeMoney(entryfee/2);
        }
    }


    void FixedUpdate()
    {
        visualizeMood();  /// Mood intorpolation to visualize the mood sphere colour

        time = Mathf.Abs(time + Time.fixedDeltaTime);
        checkingCounter = Mathf.Abs(checkingCounter + Time.fixedDeltaTime);

        manageGarbage();    /// the garbage he is holding logic

        if(status == States.Idle){
            if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) anim.Play("Idle");

            int chance = Random.Range(0,4);
            //
            if(this.mood <= 0){
                leavePark();
            }else if(hunger == 0 && !needsMoney ){ // this as well need to be dynamic... if he is hungry and has enough money, choose restaurants first
                chooseRestaurants();
                
            }else if(hunger > 0 && !needsMoney){// if he has money and not feel hungry, choose a game.
                if(chance == 0) //wonderAround
                    wonderAround();
                else
                    chooseGame();
            }
            else if(needsMoney){      // if he doesnot have money, chooseATM. Bug: IF there is nothing no building in the game, they would choose ATM 
                chooseATM();  
            }
            

            
        }else if(status == States.Walking){
            if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")) anim.Play("Walk");
            if(!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) ///desetination reached.
                {interactWithRide();}
            else    // check if path is still available
                { 
                    if( ((int)checkingCounter + 1) % checkingPathTime == 0) { // check if the path exists every some time interval.
                        //Debug.Log("checking");
                        checkingCounter = 0f;
                        pathStillExists();
                    }
                } // checking every 5 seconds 
        }                 
    }

//__________________________________________ Not Mono Bahavior stuff_____________________
    

    /*
    * Using interpoolation to update the visual effect of the Mood sphere on the guest Model.
    */    
    void visualizeMood(){
        float mood_ratio = (float)this.mood / (float)this.maxMood;                                           // the mood ratio from the full mood.
        Color c =  new Color((255f* (1-mood_ratio))/255f , (255f * mood_ratio) /255f ,  0, 120f/255f);      // using interpolation to determine the ratio of the colours
        mood_object.GetComponent<Renderer>().material.color = c;
    }

    /*
    * It manages the garbage that the guest currently have
    * 1) checks if there are trashcans. If there is none, throw on the ground
    * 2) If trashcans are found. Then find a path to the closest one to the Guest, and it checks if it is in range. if in range go to it, otherwise throw on ground
    */
    private void manageGarbage(){
        if(!navMeshAgent.isOnNavMesh || !gameObject.activeSelf) return;   
        int chance = Random.Range(0,garbage_chance); 
        
        if(chance == 0 && 
            (int)time >= droppingTime && (int)time % droppingTime == 0 
            && gameObject.activeSelf && dirts.Count > 0){

            GameObject[] buildings = GameObject.FindGameObjectsWithTag("Trashcan"); //get all trashcans

            if(buildings.Length == 0){ // no trashcans .. throw on ground
                Instantiate(dirts.Dequeue(), new Vector3(transform.position.x, 0.8f, transform.position.z), Quaternion.identity);
                return;}
            
            /*Minimum search algorithm*/
            int minI = 0;
            int side_Ind = 0;
            NavMeshPath minPath = new NavMeshPath();
            Transform sides = buildings[minI].transform.Find("Sides");
            int sidesCount = sides.childCount;

            float minPathCalc = float.PositiveInfinity;


            /*Initializing minPath and minPathCalc*/
            while( minI < buildings.Length && minPath.status != NavMeshPathStatus.PathComplete ){    //search first available path to trashcan:
                sides = buildings[minI].transform.Find("Sides");    //getting the sides of teh gameObject
                navMeshAgent.CalculatePath(sides.GetChild(0).position, minPath); //calculate first side.
                int j = 1;
                while(j < sidesCount && minPath.status != NavMeshPathStatus.PathComplete){ /// Gets a good side on the road
                    minPath.ClearCorners();
                    navMeshAgent.CalculatePath( sides.GetChild(j).position, minPath);
                    j++;
                }

                if(minPath.status == NavMeshPathStatus.PathComplete){ // if we found a good side. then we assign the minPathCalc to it
                    side_Ind = j-1;
                    minPathCalc = getPathLength(minPath);
                }
                minI++;
            }
            if( minPath.status != NavMeshPathStatus.PathComplete) return;
            minI--; //returning it back to index

            /*Looking other values if smaller value found*/
            for(int i = minI + 1 ; i < buildings.Length; i++){ // seek the closest trash can using minimum search algorithm
                
                NavMeshPath path = new NavMeshPath();
                sides = buildings[i].transform.Find("Sides");                    //getting the sides of teh gameObject
                navMeshAgent.CalculatePath(sides.GetChild(0).position, path);   //calculate first side.
                int j = 1;
                while(j < sidesCount && path.status != NavMeshPathStatus.PathComplete){ /// Gets a good side on the road
                    path.ClearCorners();
                    navMeshAgent.CalculatePath( sides.GetChild(j).position, path);
                    j++;
                }

                if(path.status == NavMeshPathStatus.PathComplete){ // if we found a good side. then we assign the minPathCalc to it
                    float pathLength = getPathLength(path);
                    if(pathLength < minPathCalc){
                        minPath = path;
                        side_Ind = j;
                        minPathCalc = pathLength;
                        minI = i;
                    }
                }
            }

            /*Minimum search result computation*/
            if(minI < buildings.Length  &&
                    minPathCalc <= trashcan_range  // trashcan in range
                    ) 
            {
                Transform tr =buildings[minI].transform;
                this.building = buildings[minI];
                navMeshAgent.SetDestination(tr.Find("Sides").GetChild(side_Ind - 1).position);
                status = States.Walking;
                anim.Play("Walk");
                //Debug.Log("We are walking");
            }else{
                Instantiate(dirts.Dequeue(), new Vector3(transform.position.x,  0.8f, transform.position.z), Quaternion.identity);
                Debug.Log(new Vector3(transform.position.x,  0.8f, transform.position.z));
            }
           
        }
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


//________________________________ Choosing Games, ATMs and restaurant area. And interacting with them.___________________________
    
    
    /*
    * It finds the main park entrance and set the destination of the NPC to that entrance
    */
    void leavePark(){
        this.building = GameObject.Find("gate_fbx").transform.Find("gate_main").gameObject;
        if(this.building){
            NavMeshPath path = new NavMeshPath();          
            navMeshAgent.CalculatePath(building.transform.position, path);     

            if (path.status != NavMeshPathStatus.PathComplete )
                {navMeshAgent.ResetPath();} /// if only no path found to the entrance
            else
            {
                ///path found 
                navMeshAgent.SetDestination(building.transform.position);
                status = States.Walking;
                anim.Play("Walk");
            }
        }
    }


    /*
    * Wondering around.
    */
    void wonderAround(){             
        if(!navMeshAgent.isOnNavMesh || !gameObject.activeSelf) return;        
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Path_Block");           //get all buildings
        if(buildings.Length == 0){
            return;
        }

        int index = Random.Range(0, buildings.Length);                                    ///random building index.
        this.building = buildings[index];       
        
        NavMeshPath path = new NavMeshPath();          
        navMeshAgent.CalculatePath(building.transform.position, path);     

        if (path.status != NavMeshPathStatus.PathComplete )
            {navMeshAgent.ResetPath();} /// if only no path found to the entrance
        else
        {
            ///path found 
            navMeshAgent.SetDestination(building.transform.position);
            if( (navMeshAgent.destination - gameObject.transform.position).sqrMagnitude <= 1f ) return;
            status = States.Walking;
            anim.Play("Walk");
        }
    }

  

    /**
    * Chosse a random building from all the building, get its entrance,
    * and checks if there is a valid path to there. Then set the destination of the NPC to that building.
    */
    private void chooseGame(){
        if(!navMeshAgent.isOnNavMesh || !gameObject.activeSelf) return;   
        NavMeshPath path = new NavMeshPath();                                             /// Used later to store the path.
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");           //get all buildings
        if(buildings.Length == 0){
            return;
        }

        int index = Random.Range(0, buildings.Length);                                    ///random building index.
        this.building = buildings[index];
        RideManager rm = this.building.GetComponent<RideManager>();
        if(rm && !(rm.getState() == GameStates.IDLE || rm.getState() == GameStates.RUNNING) ){   // if the game is availabe to play
            return;
        }          
        
        Transform pos = building.transform.Find("Entrance");                              ///Finding the entrance.
        if (pos)
        {   
            navMeshAgent.CalculatePath(pos.position, path);                                   /// Path to entrance exists
            if (path.status != NavMeshPathStatus.PathComplete)
                {navMeshAgent.ResetPath();} /// if only no path found to the entrance
            else
            {
                ///path found 
                navMeshAgent.SetDestination(pos.position);
                status = States.Walking;
                anim.Play("Walk");
            }
        }
    }


    /*
    * Choose the closest ATM and set it as the path
    */
    private void chooseATM(){
        if(!navMeshAgent || !gameObject.activeSelf) return;   
        GameObject[] atms = GameObject.FindGameObjectsWithTag("ATM");                     ///get all buildings
        if(atms.Length == 0){
            return;
        }

        /*Minimum search*/
        int minI = 0;
        NavMeshPath minPath = new NavMeshPath();
        
        /*Finding First Reachable ATM*/
        while(minI < atms.Length && minPath.status != NavMeshPathStatus.PathComplete){
            Transform tempPos = atms[minI].transform.Find("Entrance");
            navMeshAgent.CalculatePath(tempPos.position, minPath);
            minI++;
        }

        if(minPath.status != NavMeshPathStatus.PathComplete) return;       // if no valid path was found, return
        minI--; // returning it back to index.

        float minPathLength = getPathLength(minPath);

        /*Minimum search algorithm*/
        for(int i = minI + 1 ; i < atms.Length ; i++){
            NavMeshPath tempPath = new NavMeshPath();
            Transform tempPos = atms[i].transform.Find("Entrance");
            navMeshAgent.CalculatePath(tempPos.position, tempPath);
            float tempPathLength = getPathLength(tempPath);
            if(tempPath.status == NavMeshPathStatus.PathComplete && tempPathLength < minPathLength){
                minPath = tempPath;
                minPathLength = tempPathLength;
                minI = i;
            }
        }

        // if we reached an ending
        if (minI < atms.Length)
        {   
            this.building = atms[minI];
            Transform atmTr = atms[minI].transform.Find("Entrance");
            navMeshAgent.SetDestination(atmTr.position);
            status = States.Walking;
            anim.Play("Walk");
        }
    }


    /* 
    * Choosing the closest restaurant and set it as the distenation
    */
    private  void chooseRestaurants(){
        if(!navMeshAgent || !gameObject.activeSelf) return;   

        GameObject[] restaurants = GameObject.FindGameObjectsWithTag("Restaurant");                     ///get all buildings
        if(restaurants.Length == 0){
            return;
        }

        /*Minimum search*/
        int minI = 0;
        NavMeshPath minPath = new NavMeshPath();
        
        /*Finding First Reachable ATM*/
        while(minI < restaurants.Length && minPath.status != NavMeshPathStatus.PathComplete){
            Transform tempPos = restaurants[minI].transform.Find("Entrance");
            navMeshAgent.CalculatePath(tempPos.position, minPath);
            minI++;
        }
        if(minPath.status != NavMeshPathStatus.PathComplete) return;       // if no valid path was found, return
        minI--; // returning it back to index.

        float minPathLength = getPathLength(minPath);

        /*Minimum search algorithm*/
        for(int i = minI + 1 ; i < restaurants.Length ; i++){
            NavMeshPath tempPath = new NavMeshPath();
            Transform tempPos = restaurants[i].transform.Find("Entrance");
            navMeshAgent.CalculatePath(tempPos.position, tempPath);
            float tempPathLength = getPathLength(tempPath);
            if(tempPath.status == NavMeshPathStatus.PathComplete && tempPathLength < minPathLength){
                minPath = tempPath;
                minPathLength = tempPathLength;
                minI = i;
            }
        }

        // if we reached an ending
        if (minI < restaurants.Length)
        {   
            this.building = restaurants[minI];
            Transform restaurantTr = restaurants[minI].transform.Find("Entrance");
            navMeshAgent.SetDestination(restaurantTr.position);
            status = States.Walking;
            anim.Play("Walk");
        }
    }


//______________________________ Main interact funciton. Called when guest reaches distenation


    /*
    * A function that is called in intervals to check if the path to the desired position is still an active path.
    */
    void pathStillExists(){
        if(!this.building){  /// if building reference is null ( destroyed maybe )
            this.status = States.Idle;
            anim.Play("Idle");
            return;
        }


        NavMeshPath tempPath = new NavMeshPath();
        Transform tempPos = building.transform;
        if(this.building.CompareTag("Path_Block")) {tempPos = building.transform;}
        else if(this.building.name == "gate_main" ) {tempPos = building.transform;}
        else if(building.transform.Find("Entrance")) { tempPos = building.transform.Find("Entrance");}
        else if(this.building.CompareTag("Trashcan")) { // since trashcan has 4 entrances .. and is handled in realtime manageGarbage() if a path was cut to the garbage, it will just resume the normal bahavior of the guest.
            this.building = null;
            this.status = States.Idle;
            anim.Play("Idle");
            return;
        }

        navMeshAgent.CalculatePath(tempPos.position, tempPath);
        
        if(tempPath.status == NavMeshPathStatus.PathComplete){
            navMeshAgent.SetDestination(tempPos.position); /// changing directions
        }else{ /// path does not exist anymore to that building
            this.building = null;
            this.status = States.Idle;
            anim.Play("Idle");
            return;
        }
    }


    /**
    * Interact with the desired building. ATMs or Rides
    *
    */
    private void interactWithRide(){
        if(this.building == null){
            this.status = States.Idle;
            anim.Play("Idle");
            return;
        }


        if(this.building.CompareTag("ATM")){
            ATM_Manager atm_script = building.GetComponent<ATM_Manager>();
            this.money += atm_script.withdraw();
            this.status = States.Idle;
            anim.Play("Idle");
            this.needsMoney = false;
            //building = null;
            return;
        }
        
        if(this.building.CompareTag("Trashcan")){
            dirts.Clear();
            this.status = States.Idle;
            anim.Play("Idle");
            return;
        }

        if(this.building.CompareTag("Building")){
             RideManager rm = building.GetComponent<RideManager>();
             Game_Specific_Script g_script = building.GetComponent<Game_Specific_Script>();
             if(g_script && g_script.getGameMoney() > this.money){ // the needed money is more than the guest money
                subMood(60);                // sbutracting mood
                this.needsMoney = true;     // needs more money
                this.status = States.Idle;
                anim.Play("Idle");
                return;
             }
             if(rm){
                DynamicUI du = GameObject.Find("DynamicUI").GetComponent<DynamicUI>(); 
                du.changeMoney(g_script.getGameMoney() / 2);                /// adding half the game money to the player. The other half is for running fee
                //Debug.Log("The Ride money is: " + g_script.getGameMoney());
                rm.addPlayer(gameObject);
            }
            return;
        }

         if(this.building.CompareTag("Restaurant")){
            int restaurantFee = GameObject.Find("DynamicUI").GetComponent<DynamicUI>().GetEntranceFee(); 
            if(this.money < restaurantFee){      // restaurant money is 10 ? 
                this.needsMoney = true;
                subMood(40);                    // sbutracting mood
            }
            else{
                this.hunger = this.fullStomach;
                this.money -= restaurantFee;     // make it dynamic so timur can sleep at night
                addGarbage(1);
            }
            this.status = States.Idle;
            anim.Play("Idle");
            return;
        }

        if(this.building.CompareTag("Path_Block")){
            this.status = States.Idle;
            anim.Play("Idle");
            return;
        }

        if(this.building.name == "gate_main"){
            this.is_destroyed = true;
            GameObject.Find("DynamicUI").GetComponent<DynamicUI>().changeGuestAccount(-1);  // change the guest account in UI
            Destroy(gameObject);
            return;
        }

    }


///________________________________ public Functions area__________________________

    /// this should be called from the building after it is done playing. The building should call all the guests that are in it and call this function.
    public void DonePlaying(){ 
        status = States.Idle;

        //hunger -= Random.Range(20,40);
        Game_Specific_Script re = this.building.GetComponent<Game_Specific_Script>();
        if(re)
            this.money -= re.getGameMoney();

        gameObject.SetActive(true);
        anim.Play("Idle");
        this.hunger -= 5;

        building = null;
    }

    



    /*
    * Adding dirt to the guest. The garbage will be thrown in order after some time..
    */
    public void addGarbage(int amount){
        GameObject gbg = GameObject.Find("GarbageSystem");

        for(int i = 0 ; i < amount ; i++){
            
            GarbageManager gm = gbg.GetComponent<GarbageManager>();
            if(gm){
                GameObject dirt = gm.create();  
                //Debug.Log("adding dirt");
                dirts.Enqueue(dirt);
            }  
        }
    }



    /*
    * The ride manager calls this function if the mood of the guest drops to 0 while he is queueing.
    * It just removes it from the game queue.
    */
    public void leaveQueue(){
        gameObject.SetActive(true); /// in case the mood got subtracted from a game
        anim.Play("Idle");
        building = null;
        status = States.Idle;
    }
    /*
    * reduces the mood of the NPC by the given amount.
    */
    public void subMood(int mood){
        this.mood = this.mood - mood;
        if(this.mood <= 0){
            this.mood = 0;
            is_destroyed = true;  /// destroyed moodly and emotionaly .. or in other words.. the guest has decided to leave!
        }else if(this.mood > maxMood && is_destroyed == false){
            this.mood = maxMood;
        }
    }

    //______________ GETTERS AREA
    public bool isDestroyed(){return this.is_destroyed;}


    public void shiftToIdle(){
        this.status = States.Idle;
        anim.Play("Idle");
    }

    /*Called when the guest is queueing and something bad happen to the game*/
    public void kickQueuer(){
        this.status = States.Idle;
        this.mood -= 10; // for being kicked out
        gameObject.SetActive(true);
        anim.Play("Idle");
    }


    /* Thief successfuly steals money from the guest*/
    public void stealMoney(int amount){
        if(money <= 0) return;
        subMood(5);         // subtract 5 from the mood if they got stolen
        money -= amount;    // subtract the amount from the money
        if(money <= 0) {
            money = 0;
            anim.Play("Idle");
            this.status = States.Idle; // if no money .. set it to idle.
        }
    }



    /*____________________________________ Trigger handlers __________________________________ */

    /*
    * handles dirt collision with the radius of the trigger collider attached to the gameobject
    */
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Garbage"){
            subMood(1); 
        }
    }
}


