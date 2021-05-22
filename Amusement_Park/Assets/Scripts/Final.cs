using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Script for grouping built objects in the scene and giving them a destroy functionality
 */
public class Final : MonoBehaviour
{
    private BuildSystem buildSystem;//used to check if we are in destroy mode
    public Material badMat;//red material

    private Material[,] originalMats;//store original material of the GameObject the script is attached to
    private bool entered = false;

    public Game_Specific_Script gs;//used to obtain the price of the GameObject the script is attached to
    public DynamicUI dynamicUIScript;//used to keep track of UI variables

    private void Start()
    {
        buildSystem = GameObject.FindObjectOfType<BuildSystem>();
        //Game_Specific_Script gameScript = GetComponent<Game_Specific_Script>();   /**/
        
        //storing the original materials of the object
        MeshRenderer[] originalMR = this.gameObject.GetComponentsInChildren<MeshRenderer>();
        originalMats = new Material[originalMR.Length, 100];
        for (int i = 0; i < originalMR.Length; i++)//loop through all the renderers in itself and children
        {
            Material[] temp = originalMR[i].materials;
            for (int j = 0; j < temp.Length; j++)
                originalMats[i, j] = temp[j];//list all the materials
        }
        gs = GetComponent<Game_Specific_Script>();
        int gamePrice = gs.getGameMoney();
        dynamicUIScript = GameObject.Find("DynamicUI").GetComponent<DynamicUI>();
        dynamicUIScript.changeMoney(-gamePrice);
        if(gs.game_t == GamesType.POLICESTATION) { dynamicUIScript.changeGuardLimit(5); } //add 5 guards space for each security station build
    }

    //Detect if the Cursor starts to pass over the GameObject and replace the gameobject's materials to badMat
    private void OnMouseEnter()
    {
        if (buildSystem.isDestroying)
        {
            entered = true;
            foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())//loop through all the renderers in itself and children
            {
                Material[] m = r.materials;//list all the materials
                for (int i = 0; i < m.Length; i++)//change them all
                {
                    m[i] = badMat;
                }
                r.materials = m;//replace the materials of the object
            }
        }
    }

    //Detect when Cursor leaves the GameObject and restore the gameobject's original materials
    private void OnMouseExit()
    {
        if (entered) //we check if it needs to be restored
        {
            MeshRenderer[] r = GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < r.Length; i++)
            {
                Material[] m = r[i].materials;//list all the materials
                for (int j = 0; j < m.Length; j++)//change back to original materials of the object
                {
                    m[j] = originalMats[i, j];
                }
                r[i].materials = m;//replace the materials of the object
            }
            entered = false;
        }

    }
    
}
