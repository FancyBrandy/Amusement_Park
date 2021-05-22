using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace miao
{
    public class MenuManager : MonoBehaviour
    {
       // get the game object of different menu
        public GameObject buildMenu;
        public GameObject hireMenu;
        public BuildSystem buildSystem;
        public GameObject destroyButton;
        public GameObject feeMenu;

        //variable to determine if the menu appear
        private bool hireMenuAppeared = false;
        private bool buildMenuAppeared = false;
        private bool feeMenuAppeared=false;
        

        public int PriceRest=100;
        public int PriceEntry=10;
        // when game starts, all menu dont appear
        void Start()
        {
            buildMenu.SetActive(false);
            hireMenu.SetActive(false);
            feeMenu.SetActive(false);
        }


        //for game button
        public void loadStartScene()
        {
            SceneManager.LoadScene("MainMenu");
        }

        //for build button

        public void activeBuildMenu()
        {

            setNomal("Build");
            if (buildMenuAppeared)
            {
                buildMenu.SetActive(false);
            }
            else
            {
                buildMenu.SetActive(true);
            }
            buildMenuAppeared = !buildMenuAppeared;
        }

        //for destroy button
        public void destroyMeodel()
        {
            setNomal("Destroy");
            buildSystem.ToggleDestroyMode();

        }

        //for hire button

        public void ReadPriceRest(int r)
        {
            PriceRest=r;
            Debug.Log("Now the fee for the restaurant has been changed");
        }
      
        public void ReadPriceEntry (int r)
        {
            PriceEntry=r;
            Debug.Log("Now the fee for the entry has been changed");
        }

        // make hire Menu active
        public void activehireMenu()
        {
            setNomal("Hire");
            if (hireMenuAppeared)
            {
                hireMenu.SetActive(false);
            }
            else
            {
                hireMenu.SetActive(true);

            }
            hireMenuAppeared = !hireMenuAppeared;
        }


        // make fee Menu active
        public void activeFeeMenu()
        {
            setNomal("Fee");
            if (feeMenuAppeared)
            {
                feeMenu.SetActive(false);
            }
            else
            {
                feeMenu.SetActive(true);

            }
            feeMenuAppeared = !feeMenuAppeared;
        }


        // take action on others menu(inactive them) when a menu is active
        public void setNomal(string choice)
        {
          
            if (choice == "Build")
            {
                hireMenuAppeared = false;
                hireMenu.SetActive(false);
                feeMenuAppeared = false;
                feeMenu.SetActive(false);
             
            }
            else if (choice == "Hire")
            {
                buildMenuAppeared = false;
                buildMenu.SetActive(false);
                feeMenuAppeared = false;
                feeMenu.SetActive(false);
            
            }
            else if (choice == "Destroy")
            {
                hireMenuAppeared=false;
                hireMenu.SetActive(false);
                buildMenuAppeared = false;
                buildMenu.SetActive(false);
                feeMenuAppeared = false;
                feeMenu.SetActive(false);
            }
            else if (choice == "Fee")
            {
                hireMenuAppeared=false;
                hireMenu.SetActive(false);
                buildMenuAppeared = false;
                buildMenu.SetActive(false);
            }
        }
    }

}