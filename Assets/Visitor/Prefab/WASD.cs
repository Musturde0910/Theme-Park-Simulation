using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WanderingVisitor : MonoBehaviour
{
    public NavMeshAgent agent;

    private bool isLeaving = false;

    void Start()
    {
        StartCoroutine(StopMovingAfterDelay(3f));
    }

    void Update()
    {
        // If the visitor is leaving, check if they reached the spawn location
        if (isLeaving && !agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
        {
            Destroy(gameObject);
        }

        // Wander around if not leaving
        if (!isLeaving)
        {
            WanderAround();
        }
    }

    IEnumerator StopMovingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // After the delay, stop the visitor's movement and start wandering
        StartWandering();
    }

    private void StartWandering()
    {
        StartCoroutine(Wander());
    }

    private void WanderAround()
    {
        // Implement wandering behavior here
        // For example, set a random destination within a certain radius
        if (!agent.hasPath)
        {
            Vector3 randomDestination = Random.insideUnitSphere * 10f; // Adjust the radius as needed
            randomDestination.y = 0f; // Ensure the destination is on the same level
            agent.SetDestination(transform.position + randomDestination);
        }
    }

    IEnumerator Wander()
    {
        while (!isLeaving)
        {
            // Wait for a random duration before setting a new destination
            yield return new WaitForSeconds(Random.Range(3f, 10f));

            // Set a new random destination
            WanderAround();
        }
    }

    public void LeavePark(Vector3 leaveDestination)
    {
        StartCoroutine(LeaveAfterDelay(leaveDestination));
    }

    IEnumerator LeaveAfterDelay(Vector3 leaveDestination)
    {
        yield return new WaitForSeconds(Random.Range(10f, 20f)); // Adjust the leave delay as needed

        // Set the destination to leave the park
        agent.SetDestination(leaveDestination);

        // Optionally: Disable other components (e.g., scripts, renderers) to make the visitor "disappear"
        // Disable any renderer or script that makes the visitor visible or active.
        // For example: gameObject.GetComponent<Renderer>().enabled = false;

        // Wait until the visitor reaches the destination before destroying
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        // Destroy the visitor object
        Destroy(gameObject);
    }


    public void ReturnToSpawn(Vector3 spawnLocation)
    {
        // Set the destination to return to the spawn location
        agent.SetDestination(spawnLocation);

        // Mark the visitor as leaving
        isLeaving = true;
    }
}
