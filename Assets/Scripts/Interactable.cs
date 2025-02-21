using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float detectionRadius = 5f; // Radius to detect and add followers
    public float removalRadius = 7f; // Radius to remove followers
    public LayerMask followerLayer; // Layer to detect followers

    public List<GameObject> Workers = new List<GameObject>(); // Followers within proximity
    private FollowerManager followerManager;

    void Start()
    {
        FollowerManager.OnFollowerRecalled += OnFollowerRecalled;

        // Find the FollowerManager in the scene
        followerManager = FindObjectOfType<FollowerManager>();
        if (followerManager == null)
        {
            Debug.LogError("FollowerManager not found in the scene!");
        }
        else
        {
            Debug.Log("FollowerManager reference set successfully.");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the recall event to avoid memory leaks
        FollowerManager.OnFollowerRecalled -= OnFollowerRecalled;
    }

    void Update()
    {
        DetectAndAssignWorkers();
    }

    void DetectAndAssignWorkers()
    {
        // Detect followers within the detection radius
        Collider[] detectedFollowers = Physics.OverlapSphere(transform.position, detectionRadius, followerLayer);

        foreach (Collider col in detectedFollowers)
        {
            GameObject follower = col.gameObject;
            Follower followerScript = follower.GetComponent<Follower>();

            // Check if the follower is idle and not already assigned to this interactable
            if (followerScript != null && followerScript.IsIdle() && !Workers.Contains(follower))
            {
                // Set the follower to busy and assign it to this interactable
                followerScript.SetBusy(true);
                if (followerManager != null)
                {
                    followerManager.SetFollowerState(follower, FollowerState.Busy);
                    Debug.Log(follower.name + " moved to busyFollowers list.");
                }
                else
                {
                    Debug.LogError("FollowerManager reference is null!");
                }
                Workers.Add(follower);
                Debug.Log(follower.name + " has been assigned to " + gameObject.name);
            }
        }

        // Remove followers that are outside the removal radius
        Workers.RemoveAll(follower => {
            if (follower == null) return true; // Remove if the follower is destroyed

            float distance = Vector3.Distance(transform.position, follower.transform.position);
            if (distance > removalRadius)
            {
                Follower followerScript = follower.GetComponent<Follower>();
                if (followerScript != null)
                {
                    if (Workers.Contains(follower))
                    {
                        followerScript.SetBusy(false); // Reset the busy state

                        // Notify the FollowerManager to move the follower back to idleFollowers
                        if (followerManager != null)
                        {
                            followerManager.SetFollowerState(follower, FollowerState.Idle);
                        }
                    }
                }
                return true; // Remove from Workers list
            }
            return false; // Keep in Workers list
        });
    }

    void OnFollowerRecalled(GameObject follower)
    {
        // Remove the recalled follower from the Workers list if it exists
        if (Workers.Contains(follower))
        {
            Workers.Remove(follower);
            Debug.Log(follower.name + " removed from Workers list due to recall.");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw the detection and removal radii in the editor for debugging
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, removalRadius);
    }
}
