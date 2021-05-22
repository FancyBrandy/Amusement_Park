using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// A class just for initializing characters on the right positions of the games and implementing repairing

public enum GamesType{
    WHEEL, ROLLER_COASTER, AIRPLANE, CLIFF_HANGER, ENTER_PRISE,TEA_CUP, CLAW, TROIKA, WINDSEEKER, WIPEOUT,
    GARDENBUNDLE, BTREE,MAPLE, SAKURA, FOUNTAIN, RESTAURANT, HOTDOGSTAND, PATH, TRASHCAN, ATM1, ATM2, ATM3,
    POLICESTATION  /// Games
}

public class Game_Specific_Script : MonoBehaviour
{
    public GamesType game_t;



    /*Games prices*/
    private static int wheel_Price = 2200;
    private static int roller_Price = 5000;
    private static int airplane_Price = 2000;
    private static int cliff_H_Price = 1000;
    private static int enterpise_Price = 1800;
    private static int teacup_Price = 1500;
    private static int claw_Price = 1500;
    private static int troika_Price = 1200;
    private static int windseeker_Price = 3000;
    private static int wipeout_Price = 2500;
    private static int garden_Bundle = 800;
    private static int btree_Price = 400;
    private static int maple_Price = 400;
    private static int sakura_Price = 400;
    private static int fountain_Price = 500;
    private static int restaurant_Price = 1000;
    private static int hotdog_Price = 500;
    private static int path_Price = 100;
    private static int trashcan_Price = 300;
    private static int atm1_Price = 200;
    private static int atm2_Price = 200;
    private static int atm3_Price = 200;
    private static int policestation_Price = 300;

    private static int wheel_Repair = 1100;
    private static int roller_Repair = 2500;
    private static int airplane_Repair = 1000;
    private static int cliff_H_Repair = 500;
    private static int enterpise_Repair = 900;
    private static int teacup_Repair = 700;
    private static int claw_Repair = 700;
    private static int troika_Repair = 600;
    private static int windseeker_Repair = 1500;
    private static int wipeout_Repair = 1000;
   



    /** mostly the main function for distributing sprites. Switch cases can be done to check which is teh current game and redirect to a function that can add
    *   Sprites as the given model specific to each machine (Game/Ride)
    * Note in usage currently. Maybe for future use 
    */
    void addSprite(Model spriteModel){
        switch (this.game_t)
        {
            case GamesType.WHEEL:
                ferrisWheelHandler(spriteModel);
                break;
            default:
                break;  
        }   
    }

    void ferrisWheelHandler(Model spriteModel){
        ///TODO:  render this kind of model to one of its seats
        return;
    }





//____________________________ Functions regarding the current ride only ___________________________
    /*
    * It gets the seats count of the current game.
    * Called from addPlayer() in RideManager.
    */
    public int getGameSeatsCount(){
        switch(game_t){
            case GamesType.WHEEL:
                return 12;
            case GamesType.ROLLER_COASTER:
                return 10;
            case GamesType.AIRPLANE:
                return 8;
            case GamesType.CLIFF_HANGER:
                return 6;
            case GamesType.ENTER_PRISE:
                return 14;
            case GamesType.TEA_CUP:
                return 12;
            case GamesType.CLAW:
                return 18;
            case GamesType.TROIKA:
                return 15;
            case GamesType.WINDSEEKER:
                return 14; // 1 on each seat
            case GamesType.WIPEOUT:
                return 14;
            default:
                return 0;
        }  
    }



    public void getWheelFee(string s)
    {
        int par;
                int.TryParse(s,out par);
                wheel_Price=par;
                Debug.Log("now the Fee for the Wheel is: "+wheel_Price);
    }

      public void getRollerFee(string s)
    {
        int par;
                int.TryParse(s,out par);
                roller_Price=par;
                 Debug.Log("now the Fee for the Roller coaster is: "+roller_Price);
    }
    public void getAirFee(string s)
    {
        int par;
                int.TryParse(s,out par);
                airplane_Price=par;
                 Debug.Log("now the Fee for the airplane is: "+airplane_Price);
    }
             
     public void getCliffFee(string s)
    {
        int par;
                int.TryParse(s,out par);
               cliff_H_Price=par;
                 Debug.Log("now the Fee for the cliff is: "+cliff_H_Price);
    }
           

    public void getteacupFee(string s)
    {
        int par;
                int.TryParse(s,out par);
               teacup_Price=par;
                 Debug.Log("now the Fee for the teacup is: "+teacup_Price);
    }
    
    public void getenterpriceFee(string s)
    {
        int par;
                int.TryParse(s,out par);
                enterpise_Price=par;
                 Debug.Log("now the Fee for the enterprice is: "+enterpise_Price);
    }
              
     public void getclawFee(string s)
    {
        int par;
                int.TryParse(s,out par);
               claw_Price=par;
                 Debug.Log("now the Fee for the claw is: "+claw_Price);
    }


     public void gettroiFee(string s)
    {
        int par;
                int.TryParse(s,out par);
               troika_Price=par;
                 Debug.Log("now the Fee for the trioka is: "+troika_Price);
    }
            
     public void getwindFee(string s)
    {
        int par;
                int.TryParse(s,out par);
               windseeker_Price=par;
                 Debug.Log("now the Fee for the windseeker is: "+windseeker_Price);
    }
        
        public void getwipeoutFee(string s)
    {
        int par;
                int.TryParse(s,out par);
              wipeout_Price=par;
                 Debug.Log("now the Fee for the wipeout is: "+wipeout_Price);
    }
        
    

    /*
    * Gets the animator from the current game. Each game has its own place where animator hides in :)
    */
    public Animator getGameAnimator(){
        Animator ret = null;
        switch(game_t){
            case GamesType.WHEEL:
                ret = gameObject.GetComponent<Animator>();
                break;
            case GamesType.ROLLER_COASTER:
                ret = gameObject.transform.Find("RollerCoaster").GetComponent<Animator>();
                break;
            case GamesType.AIRPLANE:
                ret = gameObject.GetComponent<Animator>();
                break;
            case GamesType.CLIFF_HANGER:
                ret = gameObject.transform.Find("CliffHanger").GetComponent<Animator>();
                break;
            case GamesType.ENTER_PRISE:
                 ret = gameObject.transform.Find("EnterPrise03").GetComponent<Animator>();
                break;
            case GamesType.TEA_CUP:
                 ret = gameObject.transform.Find("TeaCup").GetComponent<Animator>();
                break;
            case GamesType.CLAW:
                ret = gameObject.transform.Find("Claw").GetComponent<Animator>();
                break;
            case GamesType.TROIKA:
                ret = gameObject.transform.Find("Troika").GetComponent<Animator>();
                break;
            case GamesType.WINDSEEKER:
                ret = gameObject.transform.Find("Windseeker").GetComponent<Animator>();
                break;
            case GamesType.WIPEOUT:
                ret = gameObject.transform.Find("Wipeout").GetComponent<Animator>();
                break;
            case GamesType.GARDENBUNDLE:
                ret = gameObject.transform.Find("Bundle").GetComponent<Animator>();
                break;
            case GamesType.BTREE:
                ret = gameObject.transform.Find("BTree_A").GetComponent<Animator>();
                break;
            case GamesType.MAPLE:
                ret = gameObject.transform.Find("Maple_A").GetComponent<Animator>();
                break;
            case GamesType.SAKURA:
                ret = gameObject.transform.Find("Sakura_A").GetComponent<Animator>();
                break;
            case GamesType.FOUNTAIN:
                ret = gameObject.transform.Find("Fountain_Final").GetComponent<Animator>();
                break;
            case GamesType.RESTAURANT:
                ret = gameObject.transform.Find("Restraurant").GetComponent<Animator>();
                break;
            case GamesType.HOTDOGSTAND:
                ret = gameObject.transform.Find("HotdogStand_Final").GetComponent<Animator>();
                break;
            case GamesType.PATH:
                ret = gameObject.transform.Find("Path_Prefab").GetComponent<Animator>();
                break;
            case GamesType.TRASHCAN:
                ret = gameObject.transform.Find("TrashCan4").GetComponent<Animator>();
                break;
            case GamesType.ATM1:
                ret = gameObject.transform.Find("ATM_body").GetComponent<Animator>();
                break;
            case GamesType.ATM2:
                ret = gameObject.transform.Find("ATM2").GetComponent<Animator>();
                break;
            case GamesType.ATM3:
                ret = gameObject.transform.Find("ATM3").GetComponent<Animator>();
                break;
            case GamesType.POLICESTATION:
                ret = gameObject.transform.Find("policaStation").GetComponent<Animator>();
                break;
            default:
                ret = gameObject.GetComponent<Animator>();
                break;
        }
        return ret;
    }


    //Get the money of the attached game to this script
    public int getGameMoney(){
        switch(game_t){
            case GamesType.WHEEL:
                return wheel_Price;
            case GamesType.ROLLER_COASTER:
                return roller_Price;
            case GamesType.AIRPLANE:
                return airplane_Price;
            case GamesType.CLIFF_HANGER:
                return cliff_H_Price;
            case GamesType.ENTER_PRISE:
                return enterpise_Price;
            case GamesType.TEA_CUP:
                return teacup_Price;
            case GamesType.CLAW:
                return claw_Price;
            case GamesType.TROIKA:
                return troika_Price;
            case GamesType.WINDSEEKER:
                return windseeker_Price;
            case GamesType.WIPEOUT:
                return wipeout_Price;
            case GamesType.GARDENBUNDLE:
                return garden_Bundle;
            case GamesType.BTREE:
                return btree_Price;
            case GamesType.MAPLE:
                return maple_Price;
            case GamesType.SAKURA:
                return sakura_Price;
            case GamesType.FOUNTAIN:
                return fountain_Price;
            case GamesType.RESTAURANT:
                return restaurant_Price;
            case GamesType.HOTDOGSTAND:
                return hotdog_Price;
            case GamesType.PATH:
                return path_Price;
            case GamesType.TRASHCAN:
                return trashcan_Price;
            case GamesType.ATM1:
                return atm1_Price;
            case GamesType.ATM2:
                return atm2_Price;
            case GamesType.ATM3:
                return atm3_Price;
            case GamesType.POLICESTATION:
                return policestation_Price;    
            default:
                return -1;
        }                
    }

      public int getRepairMoney(){
        switch(game_t){
            case GamesType.WHEEL:
                return wheel_Repair;
            case GamesType.ROLLER_COASTER:
                return roller_Repair;
            case GamesType.AIRPLANE:
                return airplane_Repair;
            case GamesType.CLIFF_HANGER:
                return cliff_H_Repair;
            case GamesType.ENTER_PRISE:
                return enterpise_Repair;
            case GamesType.TEA_CUP:
                return teacup_Repair;
            case GamesType.CLAW:
                return claw_Repair;
            case GamesType.TROIKA:
                return troika_Repair;
            case GamesType.WINDSEEKER:
                return windseeker_Repair;
            case GamesType.WIPEOUT:
                return wipeout_Repair;
            default:
                return -1;
        }                
    }


    /*
    * gets the game building time in milliseconds.
    */
    public float getGameBuildingTime(){
        switch(game_t){
            case GamesType.WHEEL:
                return 210.0f; /*3.5 mins*/
            case GamesType.ROLLER_COASTER:
                return 360.0f; /*6 mins (half day cycle in-game), 12 hours in real-life*/
            case GamesType.AIRPLANE:
                return 240.0f; /*4 mins*/
            case GamesType.CLIFF_HANGER:
                return 90.0f; /*1.5 mins*/
            case GamesType.ENTER_PRISE:
                return 120.0f; /*2 mins*/
            case GamesType.TEA_CUP:
                return 60.0f; /*1 mins*/
            case GamesType.CLAW:
                return 120.0f; /*2 mins*/
            case GamesType.TROIKA:
                return 90.0f; /*1.5 mins*/
            case GamesType.WINDSEEKER:
                return 240.0f; /*4 mins*/
            case GamesType.WIPEOUT:
                return 180.0f; /*3 mins*/
            default:
                return 0;
        }
    }

    /*
    * Returns the current game repairing time.. 
    */
    public float getGameRepairingTime(){
        switch (game_t){
            case GamesType.WHEEL:
                return 210.0f / 2f;
            case GamesType.ROLLER_COASTER:
                return 360.0f / 2f;
            case GamesType.AIRPLANE:
                return 240.0f / 2f;
            case GamesType.CLIFF_HANGER:
                return 90.0f / 2f;
            case GamesType.ENTER_PRISE:
                return 120.0f / 2f;
            case GamesType.TEA_CUP:
                return 60.0f / 2f;
            case GamesType.CLAW:
                return 120.0f / 2f;
            case GamesType.TROIKA:
                return 90.0f / 2f;
            case GamesType.WINDSEEKER:
                return 240.0f / 2f;
            case GamesType.WIPEOUT:
                return 180.0f / 2f;
            default:
                return 0;
        }
    }


//____________________________ Functions in the script ___________________________
    /*
    * static function for setting the price of the games.
    */
    public static void SetGameMoney(GamesType gametype, int money ){
        switch(gametype){
            case GamesType.WHEEL:
                wheel_Price = money;
                break;
            case GamesType.ROLLER_COASTER:
                roller_Price = money;
                break;
            case GamesType.AIRPLANE:
                airplane_Price = money;
                break;
            case GamesType.CLIFF_HANGER:
               cliff_H_Price = money;
                break;
            case GamesType.ENTER_PRISE:
                enterpise_Price = money;
                break;
            case GamesType.TEA_CUP:
                 teacup_Price = money;
                break;
            case GamesType.CLAW:
               claw_Price = money;
                break;
            case GamesType.TROIKA:
                troika_Price = money;
                break;
            case GamesType.WINDSEEKER:
               windseeker_Price = money;
                break;
            case GamesType.WIPEOUT:
                wipeout_Price = money;
                break;
            default:
                break;
        }
    }


    public void deactivate_smoke(){
        GameObject smokeE = gameObject.transform.Find("SmokeEffect").gameObject;
        smokeE.SetActive(false);
    }
    public void activate_smoke(){
        GameObject smokeE = gameObject.transform.Find("SmokeEffect").gameObject;
        smokeE.SetActive(true);
    }

    public void activate_buildingAnime(){
        GameObject rideB = gameObject.transform.Find("RideButtons").gameObject;
        rideB.SetActive(true);
        rideB.transform.Find("RepairButton").gameObject.SetActive(false);
        rideB.transform.Find("BuildingAnim").gameObject.SetActive(true);
        rideB.transform.Find("RepairingAnim").gameObject.SetActive(false);
    }

    public void activate_repairAnim(){
        GameObject rideB = gameObject.transform.Find("RideButtons").gameObject;
        rideB.SetActive(true);
        rideB.transform.Find("RepairButton").gameObject.SetActive(false);
        rideB.transform.Find("BuildingAnim").gameObject.SetActive(false);
        rideB.transform.Find("RepairingAnim").gameObject.SetActive(true);
    }

    public void activate_repairButtons(){
        GameObject rideB = gameObject.transform.Find("RideButtons").gameObject;
        rideB.SetActive(true);
        rideB.transform.Find("RepairButton").gameObject.SetActive(true);
        rideB.transform.Find("BuildingAnim").gameObject.SetActive(false);
        rideB.transform.Find("RepairingAnim").gameObject.SetActive(false);
    }

    public void deactivate_buttons(){
         GameObject rideB = gameObject.transform.Find("RideButtons").gameObject;
         rideB.SetActive(false);
    }

    
}
