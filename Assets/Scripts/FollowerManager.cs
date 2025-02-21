using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerManager : MonoBehaviour
{
    public float detectionRadius = 5f; // Radius to detect and add followers
    public float removalRadius = 7f; // Radius to remove followers
    public LayerMask followerLayer; // Layer to detect followers

    public List<GameObject> followers = new List<GameObject>(); // Followers within proximity
    public List<GameObject> idleFollowers = new List<GameObject>(); // Idle followers
    public List<GameObject> busyFollowers = new List<GameObject>(); // Busy followers (for future use)

    void Update()
    {
        DetectAndManageFollowers();

        if (Input.GetKeyDown(KeyCode.E))
        {
            ExecuteFollowerAction();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RecallIdleFollowers();
        }
    }

    void DetectAndManageFollowers()
    {
        Collider[] detectedFollowers = Physics.OverlapSphere(transform.position, removalRadius, followerLayer);
        HashSet<GameObject> detectedSet = new HashSet<GameObject>();

        foreach (Collider col in detectedFollowers)
        {
            GameObject follower = col.gameObject;
            detectedSet.Add(follower);

            // Add new followers if within detection range and not already in any list
            if (!followers.Contains(follower) && !idleFollowers.Contains(follower) && !busyFollowers.Contains(follower) &&
                Vector3.Distance(transform.position, follower.transform.position) <= detectionRadius)
            {
                followers.Add(follower);
            }
        }

        // Remove followers that are outside the removal radius
        followers.RemoveAll(follower => !detectedSet.Contains(follower));
    }

    void ExecuteFollowerAction()
    {
        if (followers.Count > 0)
        {
            GameObject follower = followers[0];
            Follower followerScript = follower.GetComponent<Follower>();
            if (followerScript != null)
            {
                followerScript.PerformAction();
                SetFollowerState(follower, FollowerState.Idle); // Set the follower to idle after performing the action
            }
        }
    }

    void RecallIdleFollowers()
    {
        // Create a copy of the idleFollowers list to avoid modifying it while iterating
        List<GameObject> idleFollowersCopy = new List<GameObject>(idleFollowers);

        foreach (GameObject follower in idleFollowersCopy)
        {
            Follower followerScript = follower.GetComponent<Follower>();
            if (followerScript != null)
            {
                followerScript.SetIdle(false); // Reset the idle state
                SetFollowerState(follower, FollowerState.Following); // Move back to the proximity list
            }
        }
    }

    void SetFollowerState(GameObject follower, FollowerState state)
    {
        // Remove the follower from all lists
        followers.Remove(follower);
        idleFollowers.Remove(follower);
        busyFollowers.Remove(follower);

        // Add the follower to the appropriate list based on the state
        switch (state)
        {
            case FollowerState.Following:
                followers.Add(follower);
                break;
            case FollowerState.Idle:
                idleFollowers.Add(follower);
                break;
            case FollowerState.Busy:
                busyFollowers.Add(follower);
                break;
        }
    }
}

public enum FollowerState
{
    Following, // Actively following the player
    Idle,      // Idle and not following the player
    Busy       // Busy with a task (for future use)
}