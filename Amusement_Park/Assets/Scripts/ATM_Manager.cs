using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATM_Manager : MonoBehaviour
{
    public int withdraw(){
        int amount = Random.Range(300, 1000); // from 300 to 1000 drawing
        return amount;
    }
}
