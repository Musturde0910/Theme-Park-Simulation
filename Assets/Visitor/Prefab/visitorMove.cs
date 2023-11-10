using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class visitorMove : MonoBehaviour
{
    public NavMeshAgent agent;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Ray moveposition = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(moveposition, out var hitInfo))
            {
                agent.SetDestination(hitInfo.point);
            }
        }
    }
}
