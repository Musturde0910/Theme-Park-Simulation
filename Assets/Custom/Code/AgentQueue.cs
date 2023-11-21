using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorQueue : MonoBehaviour
{
    public Queue queueList;

    List<Visitor> agentqueue = new List<Visitor>();

    //string counterId;
    Vector3 qdir;  // queue direction
    Vector3 qPos;
    Visitor incomingVisitor;
    float queueSpacing = 2;
  

    // Start is called before the first frame update
    void Start()
    {
        qdir = transform.forward; // the blue (z) vector
        qPos = transform.position + qdir*(agentqueue.Count+2)*queueSpacing;
        queueList.Add(this);

        Debug.Log("Queue created for "+gameObject.name+". With pos: "+qPos);

    }

    public bool Add(Visitor agent) {
        if (incomingVisitor != null) {
            return false;
        }
        incomingVisitor = agent;
        qPos.y = agent.GetPos().y;
        agent.SetDestination(qPos);
        queueSpacing = agent.GetRadius()*2f;
        qPos += qdir*queueSpacing;
        return true;        
    }

    public void Shift() {
        foreach (Visitor agent in agentqueue) {
            agent.transform.position -= qdir*queueSpacing;
        }
        qPos -= qdir*queueSpacing; 
    }

    public Visitor Pop() {
        if(queueList.Count()>0){
            Visitor agent = agentqueue[0];
            agentqueue.RemoveAt(0); 
            Shift();
            return agent;  
        }
        return null;
    }

    public int Size() {
        return agentqueue.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if (incomingVisitor == null)
            return;

        if (incomingVisitor.IsInQueue()) {
            incomingVisitor.TurnOffNavMeshAgent(qPos);
            agentqueue.Add(incomingVisitor);
            incomingVisitor = null;
            Debug.Log("Queue for "+gameObject.name+" now has size "+agentqueue.Count+". Next pos: "+qPos);
        }    
    }
}