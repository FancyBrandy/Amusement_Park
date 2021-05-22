using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageManager : MonoBehaviour
{
    public GameObject DIRT_1;
    public GameObject DIRT_2;
    public GameObject DIRT_3;
    public GameObject DIRT_4;
    public GameObject DIRT_5;
    public GameObject DIRT_6;
    public GameObject DIRT_7;


    private List<GameObject> dirts;

    void Start(){
        dirts = new List<GameObject>();
        dirts.Add(DIRT_1);
        dirts.Add(DIRT_2);
        dirts.Add(DIRT_3);
        dirts.Add(DIRT_4);
        dirts.Add(DIRT_5);
        dirts.Add(DIRT_6);
        dirts.Add(DIRT_7);
    }

    public GameObject create(){
        int index = Random.Range(0, dirts.Count);
        return dirts[index];
    }
}
