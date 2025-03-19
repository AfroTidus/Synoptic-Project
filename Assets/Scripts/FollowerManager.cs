using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerManager : MonoBehaviour
{
    public static FollowerManager Instance;

    public float detectionRadius = 5f; // Radius to detect and add followers
    public float removalRadius = 7f; // Radius to remove followers
    public LayerMask followerLayer; // Layer to detect followers

    public List<GameObject> followers = new List<GameObject>(); // Followers within proximity
    public List<GameObject> idleFollowers = new List<GameObject>(); // Idle followers
    public List<GameObject> busyFollowers = new List<GameObject>(); // Busy followers

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Subscribe to events
        EventManager.StartListening("FollowerStateChanged", OnFollowerStateChanged);
        //EventManager.StartListening("FollowerThrown", OnFollowerThrown);
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        EventManager.StopListening("FollowerStateChanged", OnFollowerStateChanged);
        //EventManager.StopListening("FollowerThrown", OnFollowerThrown);
    }

    void Update()
    {
        DetectAndManageFollowers();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ExecuteFollowerThrow();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RecallIdleFollowers();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            RecallBusyFollowers();
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

    void ExecuteFollowerThrow()
    {
        if (followers.Count > 0)
        {
            GameObject follower = followers[0];
            Follower followerScript = follower.GetComponent<Follower>();
            if (followerScript != null)
            {
                followerScript.Throw();
            }
        }
    }

    void RecallIdleFollowers()
    {
        List<GameObject> idleFollowersCopy = new List<GameObject>(idleFollowers);

        foreach (GameObject follower in idleFollowersCopy)
        {
            Follower followerScript = follower.GetComponent<Follower>();
            if (followerScript != null)
            {
                followerScript.SetIdle(false); // Reset the idle state
                SetFollowerState(follower, FollowerState.Following); // Move back to the original list
            }
        }
    }

    void RecallBusyFollowers()
    {
        List<GameObject> busyFollowersCopy = new List<GameObject>(busyFollowers);

        foreach (GameObject follower in busyFollowersCopy)
        {
            Follower followerScript = follower.GetComponent<Follower>();
            if (followerScript != null)
            {
                followerScript.SetBusy(false); // Reset the busy state
                //SetFollowerState(follower, FollowerState.Following); // Move back to original list

                // Notify Interactable that follower has been recalled
                EventManager.TriggerEvent(EventNames.FollowerRecalled, follower);

                Debug.Log(follower.name + " recalled from busy and moved to followers list.");
            }
        }
    }

    public void SetFollowerState(GameObject follower, FollowerState state)
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
                Debug.Log(follower.name + " moved to followers list.");
                break;
            case FollowerState.Idle:
                idleFollowers.Add(follower);
                Debug.Log(follower.name + " moved to idleFollowers list.");
                break;
            case FollowerState.Busy:
                busyFollowers.Add(follower);
                Debug.Log(follower.name + " moved to busyFollowers list.");
                break;
        }
    }

    private void OnFollowerStateChanged(object followerObj)
    {
        GameObject follower = (GameObject)followerObj;
        Follower followerScript = follower.GetComponent<Follower>();

        if (followerScript != null)
        {
            if (followerScript.IsIdle())
            {
                SetFollowerState(follower, FollowerState.Idle);
            }
            else if (followerScript.IsBusy())
            {
                SetFollowerState(follower, FollowerState.Busy);
            }
            else
            {
                SetFollowerState(follower, FollowerState.Following);
            }
        }
    }

    //private void OnFollowerThrown(object followerObj)
    //{
    //    GameObject follower = (GameObject)followerObj;
    //    Follower followerScript = follower.GetComponent<Follower>();

    //    if (followerScript != null)
    //    {
    //        SetFollowerState(follower, FollowerState.Idle);
    //    }
    //}
}

public enum FollowerState
{
    Following, // Actively following the player
    Idle,      // Idle and not following the player
    Busy       // Busy with a task
}