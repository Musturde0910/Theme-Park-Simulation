
// this doesnt work in Unity
//using MathNet.Numerics.Distributions;  

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

class Ride : MonoBehaviour {

    public float mean= 4;
    public float std= 2;

    public VisitorQueue customers;
    public int capacityWagon = 3;
    public int wagon=1;

    Visitor currCustomer = null!;
    Wagon[] wagons;
    int cumulativeQSize = 0;
    int maxQSize = 0;
    Normal serviceGaussian;

    DateTime prev;
    const int updateRate = 1;
    long time;
    float lastUpdate;
    int counter;
    
    void Start() {
        counter =0;
        wagons= new Wagon[wagon];
        for(int i=0;i<wagon;i++){
            wagons[i]=new Wagon(i,this.transform.position);
        }
        serviceGaussian = new Normal(mean, std);
        prev = DateTime.Now;
    }

    void Update() {
        time++;
        lastUpdate += Time.deltaTime;

        if (lastUpdate > updateRate) {
            update(customers, time);
            lastUpdate = 0;
        }
    }

    public Visitor getCurrCustomer() {
        return currCustomer;
    }

    public int getCumulativeQSize() {
        return cumulativeQSize;
    }

    public int getMaxQSize() {
        return maxQSize;
    }

    public bool update(VisitorQueue agentQueue, long Time) {
        Debug.Log(name+"-1");
        int currQSize = agentQueue.Size();
        cumulativeQSize += currQSize;
        if (currQSize > maxQSize)
            maxQSize = currQSize;
        for(int i=0; i<wagon;i++){
            Wagon currwagon=wagons[i];
            if (currwagon.serverFree == false) {
                currwagon.serviceTime --;
                //Debug.Log(serviceTime+" second remaining for server "+name);


                if (currwagon.serviceTime <= 0) {
                    //Debug.Log(name + " server done at time="+Time);
                    currwagon.drop();

                    currwagon.serverFree = true;
                        // done
                }
            }
            Debug.Log("hello");
            if (currwagon.serverFree == true && agentQueue.Size() > 0) {
                currwagon.serviceTime = (int) serviceGaussian.Sample();
                Debug.Log(name+" server starts. To finish in "+currwagon.serviceTime);
                int nbpassenger =0;
                while(agentQueue.Size()>0&&nbpassenger<capacityWagon){
                    Visitor visitor=agentQueue.Pop();
                    if(visitor != null){
                        currwagon.add(visitor);
                        counter++;
                        nbpassenger++;
                    }
                }
                if (nbpassenger==0){
                    currwagon.serverFree = true;
                }else{
                    currwagon.serverFree = false;
                }
            }

            
        }
        return false;
    }


}