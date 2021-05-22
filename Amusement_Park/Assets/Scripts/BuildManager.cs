using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * Script for mapping individual buildings to hotkeys and functions to start the building process
 */
public class BuildManager : MonoBehaviour 
{
    //the preview of the rides to be built
    public GameObject pathPreview;
    public GameObject airPlanePreview;
    public GameObject cliffHangerPreview;
    public GameObject enterprisePreview;
    public GameObject ferrisWheelPreview;
    public GameObject rollerCoasterPreview;
    public GameObject teacupPreview;
    public GameObject clawPreview;
    public GameObject troikaPreview;
    public GameObject windSeekerPreview;
    public GameObject wipeoutPreview;

    //ATMs
    public GameObject atm_1_preview;
    public GameObject atm_2_preview;
    public GameObject atm_3_preview;

    //TrashCan
    public GameObject trashcan;

    //Securit Building
    public GameObject securityBuilding;

    //garden previews
    public GameObject maplePreview;
    public GameObject sakuraPreview;
    public GameObject btreePreview;
    public GameObject gardenBundlePreview;
    public GameObject fountainPreview;

    //Restaurants
    public GameObject hotdogPreview;
    public GameObject restaurantPreview;

    public BuildSystem buildSystem;
    //dropdown menu
    public Dropdown m_Dropdown;
    public GameObject GameList;
    public GameObject GardenList;
    public GameObject RestaurantsList;
    public GameObject MiscList;

    //hire menu
    public GameObject cleaner_preview;
    public GameObject guard_preview;
    public GameObject repairMan_preview;
    //Dynamic UI
    public DynamicUI dynamicUI;


    //Error panel
    public Text errorText;
    public GameObject errorPanel;

    // Placing buildings at the press of a button
    // the logic is that we have to set a certain amount of money for the building and construction of the rides
    // but the player has to set the entrance fee
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            buildSystem.NewBuild(pathPreview);
           
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            buildSystem.NewBuild(airPlanePreview);
           
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            buildSystem.NewBuild(cliffHangerPreview);
           
        }
   
    
        if (Input.GetKeyDown(KeyCode.E))
        {
            buildSystem.NewBuild(enterprisePreview);
           
        }
        
    
        if (Input.GetKeyDown(KeyCode.F))
        {
            buildSystem.NewBuild(ferrisWheelPreview);
           
        }
         
        if (Input.GetKeyDown(KeyCode.L))
        {
            buildSystem.NewBuild(rollerCoasterPreview);
            
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            buildSystem.NewBuild(teacupPreview);
           
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            buildSystem.NewBuild(clawPreview);
            
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            buildSystem.NewBuild(troikaPreview);
           
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            buildSystem.NewBuild(windSeekerPreview);
            
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            buildSystem.NewBuild(wipeoutPreview);
           
        }

        if (Input.GetKeyDown(KeyCode.G))// maple
        {
            buildSystem.NewBuild(maplePreview);
            
        }

        if (Input.GetKeyDown(KeyCode.U))// sakura
        {
            buildSystem.NewBuild(sakuraPreview);
            
        }

        if (Input.GetKeyDown(KeyCode.J))// blacktree
        {
            buildSystem.NewBuild(btreePreview);
            
        }        
        if (Input.GetKeyDown(KeyCode.B))//garden bundle
        {
            buildSystem.NewBuild(gardenBundlePreview);
           
        }
        if (Input.GetKeyDown(KeyCode.N))//fountain
        {
            buildSystem.NewBuild(fountainPreview);
            
        }

        /*ATMs shortcuts*/
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            buildSystem.NewBuild(atm_1_preview);
           
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            buildSystem.NewBuild(atm_2_preview);
           
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            buildSystem.NewBuild(atm_3_preview);
            
        }

        //trashcan
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            buildSystem.NewBuild(trashcan);
            
        }

        //security building
        if (Input.GetKeyDown(KeyCode.Y))
        {
            buildSystem.NewBuild(securityBuilding);
           
        }

        //enters destroy mode
        if (Input.GetKeyDown(KeyCode.D))
        {
            buildSystem.ToggleDestroyMode();
        }

    }



    
    void Start()
    {
        // m_Dropdown = GetComponent<Dropdown>();
        RestaurantsList.SetActive(false);
        GardenList.SetActive(false);
        MiscList.SetActive(false);
        errorPanel.SetActive(false);
        //Add listener for when the value of the Dropdown changes, to take action
        m_Dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(m_Dropdown);
        });

        
    }

    // logic of the appearance of different menu
    void DropdownValueChanged(Dropdown change)
    {
        // Debug.Log(change.value);
        GameList.SetActive(false);
        RestaurantsList.SetActive(false);
        GardenList.SetActive(false);
        MiscList.SetActive(false);
        switch(change.value){
            case 0:
                GameList.SetActive(true);
                break;
            case 1:
                GardenList.SetActive(true);
                break;
            case 2:
                RestaurantsList.SetActive(true);
                break;
            case 3:
                MiscList.SetActive(true);
                break;
            default:
                Debug.Log("There is a bug in dropdown select");
                break;
        }
    }
    public void buildPath(){
        Game_Specific_Script gss = pathPreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(pathPreview);
        }
    }
    
     public void buildairPlane(){
        Game_Specific_Script gss = airPlanePreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(airPlanePreview);
        }
    }
     public void buildcliffHanger(){
        Game_Specific_Script gss = cliffHangerPreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(cliffHangerPreview);
        }
    }
     public void buildenterprise(){
        Game_Specific_Script gss = enterprisePreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(enterprisePreview);
        }
    }
    public void buildferrisWheel(){
        Game_Specific_Script gss = ferrisWheelPreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(ferrisWheelPreview);
        }

    }

    public void buildrollerCoaster(){
        Game_Specific_Script gss = rollerCoasterPreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(rollerCoasterPreview);
        }
    } 

    public void buildteacup(){
        Game_Specific_Script gss = teacupPreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(teacupPreview);
        }
        
    }
    public void buildclaw(){
        Game_Specific_Script gss = clawPreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(clawPreview);
        }
    }

    public void buildtroika(){
        Game_Specific_Script gss = troikaPreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(troikaPreview);
        }
    }

    public void buildwindSeeker(){
        Game_Specific_Script gss = windSeekerPreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(windSeekerPreview);
        }
    }
    public void buildwipeout(){
        Game_Specific_Script gss = wipeoutPreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(wipeoutPreview);
        }
     }

    public void buildMaple(){
        Game_Specific_Script gss = maplePreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(maplePreview);
        }
     }
    public void buildsakura(){
        Game_Specific_Script gss = sakuraPreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(sakuraPreview);
        }
     }
    
    public void buildbtree(){
        Game_Specific_Script gss = btreePreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(btreePreview);
        }
     }
    
    public void buildRestaurant(){
        Game_Specific_Script gss = restaurantPreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(restaurantPreview);
        }
    }
    
    public void buildHotdogstand(){
        Game_Specific_Script gss = hotdogPreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(hotdogPreview);
        }
    }

    public void buildBundle(){
        Game_Specific_Script gss = gardenBundlePreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(gardenBundlePreview);
        }
    }

    public void buildTrashCan(){
        Game_Specific_Script gss = trashcan.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(trashcan);
        }
    }

    public void buildATM1(){
        Game_Specific_Script gss = atm_1_preview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(atm_1_preview);
        }
    }

    public void buildATM2(){
        Game_Specific_Script gss =atm_2_preview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(atm_2_preview);
        }
    }

    public void buildATM3(){
        Game_Specific_Script gss = atm_3_preview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(atm_3_preview);
        }
    }

    public void buildFountain(){
        Game_Specific_Script gss = fountainPreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(fountainPreview);
        }
    }


    public void buildSecurityStatoin(){
           Game_Specific_Script gss = pathPreview.GetComponent<Game_Specific_Script>();
        DynamicUI dui = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        int gamePrice = gss.getGameMoney();
        if(gamePrice <= dui.getMoney()){
            buildSystem.NewBuild(securityBuilding);
        }
    }

    // hire cleaner
    public void hireCleaner()
    {
        GameObject mainEntrance = GameObject.Find("gate_fbx").transform.Find("gate_main").gameObject;
        int hiringMoney = 100;
        if(dynamicUI.getMoney() >= hiringMoney){
            Instantiate(cleaner_preview, mainEntrance.transform.position + new Vector3(0f,1f,0f), Quaternion.identity);//appear in the front
            dynamicUI.changeMoney(-hiringMoney);// everytime we hire the cleaner it costs some money as well
            StartCoroutine(toggleErrorPanel("Hire successfully"));
        }else{
            StartCoroutine(toggleErrorPanel("Not enough money"));
        }
        
    }

    // hire a guard
    public void hireGuard()
    {
        if (dynamicUI.isFullGuard()) {  
            StartCoroutine(toggleErrorPanel("Guards are up to limit")); return; 
        }

        int hiringMoney = 100;
        GameObject mainEntrance = GameObject.Find("gate_fbx").transform.Find("gate_main").gameObject;
        if(dynamicUI.getMoney() >= hiringMoney){
            Instantiate(guard_preview, mainEntrance.transform.position + new Vector3(0f,1f,0f), Quaternion.identity);//appear in the security station
            dynamicUI.changeGuardAccount(1);
            dynamicUI.changeMoney(-hiringMoney);// everytime we hire the guard it costs some money as well
            StartCoroutine(toggleErrorPanel("Hire successfully"));
        }else{
            StartCoroutine(toggleErrorPanel("Not enough Money"));
        }
       
    }
    
    // hire a repairman 
    public void hireRepairman()
    {
        if (dynamicUI.isFullRepairman()) { StartCoroutine(toggleErrorPanel("Repairmans are up to limit")); return; }
      
        int hiringMoney = 100;
        GameObject mainEntrance = GameObject.Find("gate_fbx").transform.Find("gate_main").gameObject;
        if(dynamicUI.getMoney() >= hiringMoney){
            Instantiate(repairMan_preview, new Vector3(150, 0.2F, 1.5f), Quaternion.identity); //appear in the entrance of amusement park
            dynamicUI.changeRepairmanAccount(1);
            dynamicUI.changeMoney(-hiringMoney);// everytime we hire the repair man it costs some money as well
            StartCoroutine(toggleErrorPanel("Hire successfully"));
        }else{
            StartCoroutine(toggleErrorPanel("Not enough money"));
        }
       
    }

    // use for toggle the message panel in the screen
    public IEnumerator toggleErrorPanel(string str)
    {
        errorText.text = str;
        errorPanel.SetActive(true);
        yield return new WaitForSeconds(1);
        
        errorPanel.SetActive(false);
    }

}


