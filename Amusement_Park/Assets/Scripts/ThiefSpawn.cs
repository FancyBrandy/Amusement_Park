using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefSpawn : MonoBehaviour
{

    public GameObject thiefObject;
    float timer;
    float maxInterval;

    // Start is called before the first frame update
    void Start()
    {
        maxInterval = Random.Range(30f, 60f);
        timer = 0f;
        Instantiate(thiefObject, transform.position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        timer = timer + Time.deltaTime;
        if (timer > maxInterval)
        {
            timer = 0f;
            maxInterval = Random.Range(30f, 60f);
            Instantiate(thiefObject, transform.position, transform.rotation);
        }
    }
}
