using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WASD: MonoBehaviour
{
    public NavMeshAgent agent;

    void Start()
    {
        StartCoroutine(StopMovingAfterDelay(3f));
    }

    void Update()
    {
        // If not in the queue, wander around
        WanderAround();
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
        while (true)
        {
            // Wait for a random duration before setting a new destination
            yield return new WaitForSeconds(Random.Range(3f, 10f));

            // Set a new random destination
            WanderAround();
        }
    }
}
