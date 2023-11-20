using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wagon : MonoBehaviour
{
    List<Visitor> currCustomers;
    public bool serverFree = true;
    public int serviceTime = 0;
    Vector3 pos;
    int id;

    public Wagon(int id,Vector3 pos){
        currCustomers=new List<Visitor>();
        this.id=id;
        this.pos=pos;
    }
   
   public void add(Visitor v){
        v.GetComponent<Renderer>().material.color = Color.green;
        v.AgentCollider.enabled=false;
        v.transform.position= pos;
        currCustomers.Add(v);
        v.calculateHappy(serviceTime);
   }

   public void drop(){
        foreach (Visitor visitor in currCustomers){
            visitor.AgentCollider.enabled=true;
            visitor.transform.position=new Vector3(144,0,155);
            int index =visitor.GetNextRide();
            visitor.GotoRide(index);
        }
        this.currCustomers= new List<Visitor>();
   }
}
