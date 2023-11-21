using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;


public class Visitor : MonoBehaviour
{
    
    [Range(1f, 10f)]
    public float meanHappy = 5f;
    [Range(0f, 5f)]
    public float stdHappy = 1f;

    public float HappyValue;

    [Range(1, 99)]
    public int percentageTimid = 50;

    static string[] possibleTags = {"VisitorAdventurous", "VisitorTimid"};
    int tagIndex;

    AgentState currState;
    NavMeshAgent navagent;
    public Queue queueList;

    System.Random rnd = new System.Random();

    float waitingQueue=0.1f;


    DateTime motionT;
    Vector3 lastPosition;
    int destinationRide;

    const float neighRadius = 10;
    GameObject[] Adventurous_Ride; 
    GameObject[] Timid_Ride; 
    GameObject[] AllRide;

    Collider agentCollider;
    public Collider AgentCollider {get { return agentCollider; } }

    DateTime prev;
    const int updateRate = 1;
    long time;
    float lastUpdate;

    // Start is called before the first frame update
    void Start()
    {
        int Index = UnityEngine.Random.Range(0, 100); 
        if(Index<(100-this.percentageTimid)){
            this.tagIndex=0;
        }else{
            this.tagIndex=1;
        }
        this.tag = possibleTags[this.tagIndex];
        Debug.Log("Created a "+this.tag+" agent");
        gameObject.tag=tag;
        
        agentCollider = GetComponent<CapsuleCollider>();
        navagent = GetComponent<NavMeshAgent>();

        if (this.tagIndex == 0) {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else {
            GetComponent<Renderer>().material.color = Color.blue;
        }


        // Find all Ride with good tag
        Adventurous_Ride = GameObject.FindGameObjectsWithTag("Adventurous");
        Timid_Ride = GameObject.FindGameObjectsWithTag("Timid");
        AllRide=ConcatenateArrays(Adventurous_Ride, Timid_Ride);
        Debug.Log("Created a "+this.tag+" agent");
        int ride = GetNextRide();
        GotoRide(ride);
        currState = AgentState.Wandering;
        Normal serviceGaussian = new Normal(meanHappy, stdHappy);
        this.HappyValue = serviceGaussian.Sample();
        checkHappyvalue();

        prev=DateTime.Now;

    }

    // Update is called once per frame
    void Update()
    {
        time++;
        lastUpdate += Time.deltaTime;
        if (currState == AgentState.InQueue){
            if (lastUpdate > updateRate) {
                waitingQueue++;
                lastUpdate=0;
                return;
            }
            return;   
        }
        
        if (currState == AgentState.ToQueue) {
            if (ReachedDestination()) {
                currState = AgentState.InQueue;
                this.lastUpdate=0;
            }

            return;
        }

        // currState == Wandering
        else {
            if (ReachedDestination(10)) {
                if (this.name == "Agent-19")
                    Debug.Log("Visitor "+this.name +" is moving elsewhere");


                bool joinQ = UnityEngine.Random.value < 1;
                if (joinQ) {
                    joinQ = MoveToQueue(destinationRide);
                }

                if (!joinQ ) {
                    destinationRide = GetNextRide();
                    GotoRide(destinationRide);
                    }
                }
            }

    }

    GameObject[] ConcatenateArrays(GameObject[] array1, GameObject[] array2)
    {
        GameObject[] result = new GameObject[array1.Length + array2.Length];
        array1.CopyTo(result, 0);
        array2.CopyTo(result, array1.Length);
        return result;
    }

    int GetStall() {
        return destinationRide;
    }
    public void calculateHappy(int servicetime){
        double tempValue =1+(-0.5*(time/waitingQueue));
        if(tempValue<-1){
            HappyValue--;
        }else{
            if(tempValue>1){
                HappyValue ++;
            }else{
                HappyValue+=(float)tempValue;
            }
        }
    }

    public int GetNextRide() {
        if (AllRide.Length==0){
            Destroy(this);

        }
        if (tagIndex == 0)
        {
            float Index = UnityEngine.Random.Range(0, (2 * Adventurous_Ride.Length) + Timid_Ride.Length);

            if (Index < 2 * Adventurous_Ride.Length)
            {
                return (int)Mathf.Floor(Index / 2);
            }
            else
            {
                return (int)Mathf.Floor((Index - (2 * Adventurous_Ride.Length)) / 2);
            }
        }
        else
        {
            float Index = UnityEngine.Random.Range(0, Adventurous_Ride.Length + (2 * Timid_Ride.Length));

            if (Index >= Adventurous_Ride.Length)
            {
                return (int)Mathf.Floor((Index - Adventurous_Ride.Length) / 2 + Adventurous_Ride.Length);
            }
            else
            {
                return (int)Mathf.Floor(Index);
            }
        }
    }

    public void GotoRide(int index) {
        SetDestination(RandomStallPos(index));
        if (this.tagIndex == 0) {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        MoveFromQueue();
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


    public bool MoveToQueue(int qindex)
    {
        if (qindex<0||qindex>queueList.Count()-1){
            qindex= UnityEngine.Random.Range(0,queueList.Count());
        }
        VisitorQueue chosenQ = queueList.Get(qindex);
        bool success = chosenQ.Add(this);
        if (!success)
            return false;        
        currState = AgentState.ToQueue;
        return true;
    }

    public string MoveToQueue()
    {
        VisitorQueue chosenQ = queueList.Get(queueList.Count());
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
        Debug.Log("Created a "+this.tag+" agent"+index+"total"+AllRide.Length);
        GameObject stall = AllRide[index];
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

        GameObject ground = GameObject.Find("Cube (6)");
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

    private void checkHappyvalue(){
        if (this.HappyValue>10){
            this.HappyValue=10;
        }else if (this.HappyValue<0){
            this.HappyValue=0;
        }
    }
}