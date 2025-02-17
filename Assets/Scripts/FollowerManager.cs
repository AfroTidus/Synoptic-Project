using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerManager : MonoBehaviour
{
    public float detectionRadius = 5f; // Radius to detect and add followers
    public float removalRadius = 7f; // Radius to remove followers
    public LayerMask followerLayer; // Layer to detect followers
    public List<GameObject> followers = new List<GameObject>();

    void Update()
    {
        DetectAndManageFollowers();

        if (Input.GetKeyDown(KeyCode.E))
        {
            ExecuteFollowerAction();
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

            // Add new followers if within detection range
            if (!followers.Contains(follower) && Vector3.Distance(transform.position, follower.transform.position) <= detectionRadius)
            {
                followers.Add(follower);
            }
        }

        // Remove followers that are outside the removal radius
        followers.RemoveAll(follower => !detectedSet.Contains(follower));
    }

    public List<GameObject> GetFollowers()
    {
        return followers;
    }

    void ExecuteFollowerAction()
    {
        if (followers.Count > 0)
        {
            Follower followerScript = followers[0].GetComponent<Follower>();
            if (followerScript != null)
            {
                followerScript.PerformAction();
            }
        }
    }
}
