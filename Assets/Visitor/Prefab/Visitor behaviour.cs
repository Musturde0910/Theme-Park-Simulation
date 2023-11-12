using System.Collections;
using UnityEngine;

public class VisitorBehavior : MonoBehaviour
{
    private Vector3 leaveDestination;

    public void SetLeaveDestination(Vector3 destination)
    {
        leaveDestination = destination;
    }

    public void LeavePark()
    {
        StartCoroutine(LeaveAfterDelay());
    }

    IEnumerator LeaveAfterDelay()
    {
        yield return new WaitForSeconds(Random.Range(10f, 20f)); // Adjust the leave delay as needed

        // For simplicity, let's assume you're destroying the visitor object here
        Destroy(gameObject);
    }
}
