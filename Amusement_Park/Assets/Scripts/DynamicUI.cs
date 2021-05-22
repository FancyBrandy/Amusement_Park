using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public  class DynamicUI : MonoBehaviour
{
    // text object of UI
    public Text guardText;
    public Text repairmanText;
    public Text timeText;
    public Text moneyText;
    public Text guestText;

    // static status of the UI system
    static private int guestAccount;
    static private int guestLimit;
    static private int guardAccount;
    static private int guardLimit;
    static private int repairmanAccount;
    static private int repairmanLimit;
    static private int money;
    

    //guests prefabs
    public GameObject remy;
    public GameObject stefani;
    public GameObject sewer_entrance;

    //Spawning timer for guests
    private float spawnTimer = 0f;
    private const float spawnTime = 30f; // every half a min

    //timer
    static private float time;
    static private int hour;
    static private int minutes;

    //the entrance fee
    static private int EntranceFee = 200;       // default entry fee
    static private int restaurantFee = 400;     // default restaurant fee


    void Start()
    {
        //initial the values
        guestAccount = GameObject.FindGameObjectsWithTag("Guest").Length;   // find how many guests in teh scene
        guestLimit = 0;       // will rely on the games. Ridemanager.start() will increase it
        guardAccount = GameObject.FindGameObjectsWithTag("Guard").Length;
        guardLimit = 0;
        repairmanAccount = GameObject.FindGameObjectsWithTag("Repairman").Length;
        repairmanLimit = 5;
        money = 5000;
        time = 0;
        hour = 5;
        //present the values
        presentData();
    }


    /* update time */
    void FixedUpdate()
    {

        time += Time.fixedDeltaTime * 2;
        spawnTimer += Time.fixedDeltaTime;


        minutes = (int)time % 60;
        hour = ((int)time / 60 + 7 ) % 24;

        /*Spawning guests funcitonality*/
        if(spawnTimer > spawnTime){
            spawnTimer = 0;
            spawnGuest();
        }

        // spawn a theif den entrance every 24 hours at 11 pm at night 
        if((int)hour == 23 && minutes == 0){
            spawnDen();
        }

        presentData();
    }




    /*
    * Spawn the Thief Den. Called once every 24 hours. If there is a thief sewer already exists. The new sewer_den will not get spawned
    */
    private void spawnDen(){
        /*check if there is a sewer already somewhere in the scene*/
        GameObject[] existing_sewer = GameObject.FindGameObjectsWithTag("Sewer_Entrance");
        if(existing_sewer.Length >= 1) return;

        /*Get a random path block to place the sewer on*/
        GameObject[] pathBlocks = GameObject.FindGameObjectsWithTag("Path_Block");
        int index_of_parent = Random.Range(0, pathBlocks.Length);
        
        /*Spawning the sewer on the chosen path*/
        GameObject go = Instantiate(sewer_entrance,pathBlocks[index_of_parent].transform.position + new Vector3(0f,0.05f,0f) ,Quaternion.identity ,pathBlocks[index_of_parent].transform);
        go.transform.localScale = new Vector3(0.0005f,0.0055f,0.0005f);
    }


    /*
    * Spawn the guest by equal chances. Called whenever a guest needs to be spawned
    */
    private void spawnGuest(){
        if(guestAccount < guestLimit){
            int chance = Random.Range(0,2);
            if(chance == 0 ){
                GameObject mainEntrance = GameObject.Find("gate_fbx").transform.Find("gate_main").gameObject;
                Instantiate(remy , mainEntrance.transform.position + new Vector3(0f,1f,0f), Quaternion.identity);
                guestAccount++;
            }else{
                GameObject mainEntrance = GameObject.Find("gate_fbx").transform.Find("gate_main").gameObject;
                Instantiate(stefani, mainEntrance.transform.position + new Vector3(0f,1f,0f), Quaternion.identity);
                guestAccount++;
            }
        }
    }

    /*  present the data on the UI */
    private void presentData()
    {   
        if(hour<10 && minutes<10){
            timeText.text = "0" + hour + ":" + "0" + minutes;
        }
        else if (hour < 10)
        {
            timeText.text = "0" + hour + ":" + minutes;
        }else if (minutes < 10)
        {
            timeText.text = hour + ":" +"0"+ minutes;
        }
        else
        {
            timeText.text = hour + ":" + + minutes;
        }
        guardText.text = guardAccount + " / " + guardLimit;
        repairmanText.text = repairmanAccount + " / " + repairmanLimit;
        guestText.text = guestAccount + "  / " + guestLimit;
        moneyText.text = money.ToString();

    }

    /* used for changing of guard account */
    public void changeGuardAccount(int x)
    {
        guardAccount += x;
    }

    /* used for changing of guard limit */
    public void changeGuardLimit(int x)
    {
        guardLimit += x;
    }

    /* used for changing of repairman account */
    public void changeRepairmanAccount(int x) {
        repairmanAccount += x;
    }

    /* used for changing of repairman limit */
    public void changeRepairmanLimit(int x)
    {
        repairmanLimit += x;
    }

    /* used for changing of guest account */
    public void changeGuestAccount(int x)
    {
        guestAccount += x;
    }

    /* used for changing of money */
    public void changeMoney(int x) {
        money += x;    
    }

    /* used for changing of guest limit */
    public void changeGuestLimit(int x){
        guestLimit += x;
        if(guestLimit < 0 ) guestLimit = 0; // will never happen but just in case
    }

    /* get the current money */
    public int getMoney(){return money;}
    /* check if the guard is up to limit */
    public bool isFullGuard() { return guardAccount >= guardLimit; }
    /* check if repairman is up to limit */
    public bool isFullRepairman() { return repairmanAccount >= repairmanLimit; }
    /* Set the entry fee of the park */
    public void SetEntranceFee(string s) {int.TryParse(s,out EntranceFee);}
    /* Get the entry fee of the park */
    public int GetEntranceFee() {return EntranceFee;}
    /* Set the entry fee of the park */
    public void SetRestaurant(string s){int.TryParse(s,out restaurantFee);}
    /* Set the entry fee of the park */
    public int GetRestaurant(){return restaurantFee;}

}
