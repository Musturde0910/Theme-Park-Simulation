using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Crowd : MonoBehaviour
{

    [Range(1f, 10f)]
    public float meanHappy = 5f;
    [Range(0f, 5f)]
    public float stdHappy = 1f;

    public int timesimulation = 10;

    public Visitor agentPrefab;
    List<Visitor> crowd = new List<Visitor>();
 
    public int startingCount = 3;

    [Range(1f, 100f)]
    public float driveFactor = 10f;

    [Range(1f, 100f)]
    public float maxSpeed = 5f;

    [Range(1f, 10f)]
    public float neighRadius = 1.5f;


    DateTime prev;
    const int updateRate = 1;
    long time;
    float lastUpdate;
    int clock;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject ground = GameObject.Find("Cube (6)");
        Vector3 grounddim = ground.transform.localScale;
        Vector3 groundpos = ground.transform.position;
        float y = groundpos.y + grounddim.y/2;

        while (crowd.Count < startingCount) {
            var x = UnityEngine.Random.Range(groundpos.x-grounddim.x/2, groundpos.x+grounddim.x/2);
            var z = UnityEngine.Random.Range(groundpos.z-grounddim.z/2, groundpos.z+grounddim.z/2);
            Vector3 spawnPos = new Vector3(x, y, z);

            Visitor agent = Instantiate(agentPrefab, 
                                    spawnPos,
                                     Quaternion.identity);
            agent.name = "Agent-"+crowd.Count;            
            crowd.Add(agent);
            
        }

        prev = DateTime.Now;
        lastUpdate = 0;
        clock = 0;
    }

    // Update is called once per frame
    void Update()
    {
        lastUpdate += Time.deltaTime;
        if (lastUpdate > updateRate) {
            clock++;
            lastUpdate=0;
            return;
        }

        if(clock>timesimulation){
            Debug.Log("finish");
        }
        
    }

}