using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class VisitorManager : MonoBehaviour
{
    public GameObject visitorPrefab;
    public Transform spawnLocation;
    public float maxSpawnInterval = 10f;
    public float minSpawnInterval = 5f;
    public int maxVisitors = 50;
    public float timeBeforeLeaving = 30f;

    void Start()
    {
        StartCoroutine(SpawnVisitors());
    }

    IEnumerator SpawnVisitors()
    {
        while (true)
        {
            int numToSpawn = Random.Range(1, 6); // Spawn between 1 to 5 visitors at a time
            for (int i = 0; i < numToSpawn; i++)
            {
                SpawnVisitor();
            }

            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnVisitor()
    {
        if (GameObject.FindGameObjectsWithTag("Visitor").Length < maxVisitors)
        {
            GameObject visitor = Instantiate(visitorPrefab, spawnLocation.position, Quaternion.identity);
            NavMeshAgent agent = visitor.GetComponent<NavMeshAgent>();

            // Set the destination to the spawn location for leaving the park
            Vector3 leaveDestination = spawnLocation.position;
            leaveDestination.x += Random.Range(-5f, 5f); // Randomize x position a bit
            leaveDestination.z += Random.Range(-5f, 5f); // Randomize z position a bit

            WanderingVisitor wanderingVisitor = visitor.GetComponent<WanderingVisitor>();
            wanderingVisitor.LeavePark(leaveDestination);
        }
    }

    // Example: Trigger the return to spawn location when needed
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReturnAllToSpawn();
        }
    }

    void ReturnAllToSpawn()
    {
        GameObject[] visitors = GameObject.FindGameObjectsWithTag("Visitor");
        foreach (var visitor in visitors)
        {
            WanderingVisitor wanderingVisitor = visitor.GetComponent<WanderingVisitor>();
            wanderingVisitor.ReturnToSpawn(spawnLocation.position);
        }
    }
}
