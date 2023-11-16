using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;


public class Visitor : MonoBehaviour
{


    NavMeshAgent navagent;
    public Queue queueList;

    System.Random rnd = new System.Random();

    DateTime motionT;
    Vector3 lastPosition;

    DateTime mouseT;
    const float mouseTime = 5;
    AgentState currState;
    int destinationStall;

    const float neighRadius = 10;
    GameObject[] stalls; // TODO: make this static/scriptable

    Collider agentCollider;
    public Collider AgentCollider {get { return agentCollider; } }

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<CapsuleCollider>();
        navagent = GetComponent<NavMeshAgent>();

        GetComponent<Renderer>().material.color = Color.red;

        // agent chooses to go to a random stall
        stalls = GameObject.FindGameObjectsWithTag("Queue");
        Debug.Log("indexlength:"+ stalls.Length);
        int stall = GetRandomStall();
        GotoStall(stall);
        currState = AgentState.Wandering;
    }

    // Update is called once per frame
    void Update()
    {
        if (currState == AgentState.InQueue)
            return;

        if (currState == AgentState.ToQueue) {
            if (ReachedDestination()) {
                currState = AgentState.InQueue;
            }

            return;
        }

        if (currState == AgentState.FollowingMouse) {
            if (ReachedDestination()) {
                currState = AgentState.Wandering;
            }

            DateTime mouseTNow = DateTime.Now;
            var diffTime = mouseTNow - mouseT;
            if (diffTime.Seconds > mouseTime)
                currState = AgentState.Wandering;
            return;
        }

        // currState == Wandering

        if (Input.GetMouseButtonDown(1)) {
                if (rnd.Next(2) == 0)
                    return;
                
                Ray movePosition = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(movePosition, out var hitInfo)) {
                    SetDestination(hitInfo.point);
                    currState = AgentState.FollowingMouse; 
                    mouseT = DateTime.Now;       
                }
            } 
        else {
            if (ReachedDestination(10)) {
                if (this.name == "Agent-19")
                    Debug.Log("Visitor "+this.name +" is moving elsewhere");


                bool joinQ = UnityEngine.Random.value < 1;
                if (joinQ) {
                    joinQ = MoveToQueue(destinationStall);
                }

                if (!joinQ ) {
                    destinationStall = GetRandomStall();
                    GotoStall(destinationStall);
                    }
                }
            }

    }

    int GetStall() {
        return destinationStall;
    }

    int GetRandomStall() {
        
        int index = UnityEngine.Random.Range(0, stalls.Length);        
        return index;
    }

    void GotoStall(int index) {
        SetDestination(RandomStallPos(index));
    }

    int CheckGangDestination() {
        List<Visitor> agents = GetNearbyAgents(true);
        Vector3 center = Vector3.zero;
        if (agents.Count == 0) {
            return GetStall();
        }        
        
        int numStalls = stalls.Length;
        int[] vote = new int[numStalls];

        foreach (Visitor agent in agents) {
            if (agent.IsInQueue() || agent.MovingToQueue() || agent.MovingToMouse())
                continue;
            vote[agent.GetStall()] ++;
         }

        int max = 0;
        int index = 0;
        for (int i=0; i<numStalls; i++) {
            if (vote[i] > max) {
                max = vote[i];
                index = i;
            }
        }

        return index;
    }


    List<Visitor> GetNearbyAgents(bool aliketype) {
        Collider[] hitColliders = new Collider[20];
        float radius = neighRadius;
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders);

        List<Visitor> context = new List<Visitor>();
        for (int i=0; i<numColliders; i++)  {
            Collider c = hitColliders[i];
            Visitor agent = c.GetComponent<Visitor>();
            if (agent == null)
                continue;

            if (agent.CompareTag(this.tag) != aliketype)
                    continue;
                    
            if (agent.name == this.name) 
                continue;
            
            context.Add(agent);
        }

        return context;
    }

    public bool MovingToQueue() {
        return currState == AgentState.ToQueue;
    }

    public bool IsInQueue() {
        return currState == AgentState.InQueue;
    }

    public bool MovingToMouse() {
        return currState == AgentState.FollowingMouse;
    }

    public bool MoveToQueue(int qindex)
    {
        VisitorQueue chosenQ = queueList.Get(qindex);
        bool success = chosenQ.Add(this);
        if (!success)
            return false;        
        currState = AgentState.ToQueue;
        return true;
    }

    public string MoveToQueue()
    {
        VisitorQueue chosenQ = queueList.Get(0);
        bool success = chosenQ.Add(this);
        if (!success)
            return "None";
        currState = AgentState.ToQueue;
        return chosenQ.name;
    }

    public void MoveFromQueue()
    {
        currState = AgentState.Wandering;
        TurnOnNavMeshAgent();
    }

    public void SetDirection(Vector3 dir) {
        navagent.Move(dir);
    }

    public void SetDestination(Vector3 pos) {
        navagent.SetDestination(pos);
        motionT = DateTime.Now;
    }

    public float GetTimeInMotion() {
        return (DateTime.Now - motionT).Seconds;
    }

    public bool ReachedDestination() {
      return (navagent.remainingDistance <= navagent.stoppingDistance);
    }

    public bool ReachedDestination(float maxsec) {
      if (navagent.remainingDistance <= navagent.stoppingDistance) {
        return true;
      }

      if (GetTimeInMotion() > maxsec) {
        return true;
      }

      return false;
    }


    public Vector3 RandomStallPos(int index) {
        GameObject stall = stalls[index];
        Vector3 stalldim = stall.transform.localScale;
        Vector3 stallpos = stall.transform.position;
        float y = transform.position.y;

        float sign = UnityEngine.Random.value < 0.5 ? -1 : 1;

        var x = stallpos.x + sign*UnityEngine.Random.Range(stalldim.x, 2*stalldim.x);
        var z = stallpos.z + sign*UnityEngine.Random.Range(stalldim.z, 2*stalldim.z);
        Vector3 randPos = new Vector3(x, y, z);

        return randPos;
    }

    public Vector3 RandomNearPosition() {

        GameObject ground = GameObject.Find("Cube 1 (4)");
        Vector3 grounddim = ground.transform.localScale;
        Vector3 groundpos = ground.transform.position;
        float y = transform.position.y;

        var x = UnityEngine.Random.Range(groundpos.x-grounddim.x/2, groundpos.x+grounddim.x/2);
        var z = UnityEngine.Random.Range(groundpos.z-grounddim.z/2, groundpos.z+grounddim.z/2);
        Vector3 randPos = new Vector3(x, y, z);
        Vector3 rdir = randPos - transform.position;
        //rdir = Vector3.Normalize(rdir);

        return rdir; //*5;
    }


    public void SetRandomDestination() {
        SetDestination(transform.position + RandomNearPosition());
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result) {
        for (int i = 0; i < 30; i++) {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) {
                result = hit.position;
                return true;
                }
            }
        result = Vector3.zero;
        return false;
        }

    public Vector3 GetPos() {
        return transform.position;
    }

    public float GetRadius() {
        return ((CapsuleCollider)agentCollider).radius; 
    }

    public void TurnOffNavMeshAgent(Vector3 pos) {
        navagent.updatePosition = false;        
        transform.position = pos;
    }

    public void TurnOnNavMeshAgent() {
        navagent.updatePosition = true;   
    }
}