using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowd : MonoBehaviour
{
    public Visitor agentPrefab;
    List<Visitor> crowd = new List<Visitor>();
 
    public int startingCount = 3;

    [Range(1f, 100f)]
    public float driveFactor = 10f;

    [Range(1f, 100f)]
    public float maxSpeed = 5f;

    [Range(1f, 10f)]
    public float neighRadius = 1.5f;

    
    // Start is called before the first frame update
    void Start()
    {
        GameObject ground = GameObject.Find("Cube 1");
        Vector3 grounddim = ground.transform.localScale;
        Vector3 groundpos = ground.transform.position;
        float y = groundpos.y + grounddim.y/2;

        while (crowd.Count < startingCount) {
            var x = Random.Range(groundpos.x-grounddim.x/2, groundpos.x+grounddim.x/2);
            var z = Random.Range(groundpos.z-grounddim.z/2, groundpos.z+grounddim.z/2);
            Vector3 spawnPos = new Vector3(x, y, z);

            Visitor agent = Instantiate(agentPrefab, 
                                    spawnPos,
                                     Quaternion.identity);
            agent.name = "Agent-"+crowd.Count;            
            crowd.Add(agent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            for (int i=0; i<3; i++) { // try 3 times
                Visitor agent = crowd[Random.Range(0, crowd.Count)];
                if (agent.IsInQueue() == false && agent.MovingToQueue() == false) {
                    string q = agent.MoveToQueue();
                    Debug.Log("Moving agent "+agent.name+" to queue "+q);
                    break;
                }
            }
        }
    }

}