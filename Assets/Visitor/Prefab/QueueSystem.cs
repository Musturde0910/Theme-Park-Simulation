using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueSystem : MonoBehaviour
{
    public GameObject queueMemberPrefab;  // The prefab for the queue members
    public Transform queueHead;           // The position where the queue starts

    public int queueLength = 5;            // Number of members in the queue
    public float spacing = 1.5f;           // Spacing between queue members

    private List<Transform> queueMembers = new List<Transform>();

    void Start()
    {
        // Spawn queue members
        SpawnQueueMembers();
    }

    void SpawnQueueMembers()
    {
        for (int i = 0; i < queueLength; i++)
        {
            Vector3 memberPosition = queueHead.position + Vector3.forward * i * spacing;
            GameObject queueMember = Instantiate(queueMemberPrefab, memberPosition, Quaternion.identity);
            queueMembers.Add(queueMember.transform);
        }
    }
}
