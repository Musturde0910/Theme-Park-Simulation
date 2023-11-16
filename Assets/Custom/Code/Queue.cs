using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName="Behavior/Queue")]
public class Queue : ScriptableObject
{

    [System.NonSerialized]
    List<VisitorQueue> queues = new List<VisitorQueue>();

    public VisitorQueue Get(int i) {
        Debug.Log("Visitor "+i );
        return queues[i];
    }

    public int Count() {
        return queues.Count;
    }

    public void Add(VisitorQueue queue) 
    {
        queues.Add(queue);
    }
}
