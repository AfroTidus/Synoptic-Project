using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FollowerManager : MonoBehaviour
{
    public static FollowerManager Instance;

    public float detectionRadius = 5f; // Radius to detect and add followers
    public float removalRadius = 7f; // Radius to remove followers
    public LayerMask followerLayer; // Layer to detect followers

    public int maxFollowers;
    public int followerCount;
    public TextMeshProUGUI followerDisplay;
    public TextMeshProUGUI currentTypeDisplay;

    public FollowerType currentType = FollowerType.Any;

    public List<GameObject> followers = new List<GameObject>(); // Followers within proximity
    public List<GameObject> idleFollowers = new List<GameObject>(); // Idle followers
    public List<GameObject> busyFollowers = new List<GameObject>(); // Busy followers
    public List<GameObject> carryingFollowers = new List<GameObject>();

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
        EventManager.StartListening(EventNames.FollowerStateChanged, OnFollowerStateChanged);
        EventManager.StartListening(EventNames.FollowerSpawned, OnFollowerSpawned);
        EventManager.StartListening(EventNames.FollowerDeath, OnFollowerDeath);
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        EventManager.StopListening(EventNames.FollowerStateChanged, OnFollowerStateChanged);
        EventManager.StopListening(EventNames.FollowerSpawned, OnFollowerSpawned);
        EventManager.StopListening(EventNames.FollowerDeath, OnFollowerDeath);
    }

    private void Start()
    {
        currentType = FollowerType.Any;
        currentTypeDisplay.text = "Any";
        currentTypeDisplay.color = Color.white;
    }

    void Update()
    {
        DetectAndManageFollowers();
        followerDisplay.text = "Followers: " +followerCount+ "/" + maxFollowers;

        if(followerCount >= maxFollowers)
        {
            EventManager.TriggerEvent(EventNames.MaxFollowersReached, this.gameObject);
        }
        if(followerCount < maxFollowers)
        {
            EventManager.TriggerEvent(EventNames.BelowMaxFollowers, this.gameObject);
        }

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

        if (Input.GetKeyDown(KeyCode.C))
        {
            RecallCarryingFollowers();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            CommandFollowersToInteractable();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentType = FollowerType.Any;
            currentTypeDisplay.text = "Any";
            currentTypeDisplay.color = Color.white;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        { 
            currentType = FollowerType.Base;
            currentTypeDisplay.text = "Base";
            currentTypeDisplay.color = Color.black;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) 
        { 
            currentType = FollowerType.Fire;
            currentTypeDisplay.text = "Fire";
            currentTypeDisplay.color = Color.red;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4)) 
        { 
            currentType = FollowerType.Water;
            currentTypeDisplay.text = "Water";
            currentTypeDisplay.color = Color.blue;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5)) 
        { 
            currentType = FollowerType.Poison;
            currentTypeDisplay.text = "Poison";
            currentTypeDisplay.color = Color.green;
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
        var eligibleFollowers = GetFollowersOfCurrentType(followers);

        if (eligibleFollowers.Count > 0)
        {
            GameObject follower = eligibleFollowers[0];
            Follower followerScript = follower.GetComponent<Follower>();

            if (followerScript != null && !followerScript.IsCarrying())
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
                followerScript.SetIdle(false);
                SetFollowerState(follower, FollowerState.Following);
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

                // Notify Interactable that follower has been recalled
                EventManager.TriggerEvent(EventNames.FollowerRecalled, follower);

                Debug.Log(follower.name + " recalled from busy and moved to followers list.");
            }
        }
    }

    void RecallCarryingFollowers()
    {
        List<GameObject> carryingFollowersCopy = new List<GameObject>(carryingFollowers);

        foreach (GameObject follower in carryingFollowersCopy)
        {
            Follower followerScript = follower.GetComponent<Follower>();
            if (followerScript != null)
            {
                followerScript.SetCarrying(false);
                // This will trigger the CarryInteractable to release the object
                EventManager.TriggerEvent(EventNames.FollowerRecalled, follower);
            }
        }
    }

    void CommandFollowersToInteractable()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null || player.lockedInteractable == null) return;

        Vector3 targetPosition = player.lockedInteractable.transform.position;

        var followersToCommand = GetFollowersOfCurrentType(followers);

        foreach (GameObject followerObj in followersToCommand)
        {
            Follower follower = followerObj.GetComponent<Follower>();
            follower.MoveToPosition(targetPosition);
        }
    }

    private List<GameObject> GetFollowersOfCurrentType(List<GameObject> followerList)
    {
        if (currentType == FollowerType.Any)
            return new List<GameObject>(followerList);

        var filteredFollowers = new List<GameObject>();
        foreach (var follower in followerList)
        {
            var followerScript = follower.GetComponent<Follower>();
            if (followerScript != null && followerScript.GetFollowerType() == currentType)
            {
                filteredFollowers.Add(follower);
            }
        }

        return filteredFollowers;
    }

    public void SetFollowerState(GameObject follower, FollowerState state)
    {
        // Remove the follower from all lists
        followers.Remove(follower);
        idleFollowers.Remove(follower);
        busyFollowers.Remove(follower);
        carryingFollowers.Remove(follower);


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
            case FollowerState.Carrying:
                carryingFollowers.Add(follower);
                break;
            case FollowerState.Dead:
                break;
        }

        Debug.Log($"{follower.name} state changed to {state}");
    }

    private void OnFollowerStateChanged(object followerObj)
    {
        GameObject follower = (GameObject)followerObj;
        if (follower == null) return;
        Follower followerScript = follower.GetComponent<Follower>();

        if (followerScript != null)
        {
            if (followerScript.IsDead())
            {
                SetFollowerState(follower, FollowerState.Dead);
            }
            else if (followerScript.IsCarrying())
            {
                SetFollowerState(follower, FollowerState.Carrying);
            }
            else if (followerScript.IsIdle())
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

    private void OnFollowerSpawned(object T)
    {
        followerCount++;
    }

    private void OnFollowerDeath(object T)
    {
        Debug.Log("Death event received for: " + ((GameObject)T).name);
        followerCount--;
        Debug.Log("New count: " + followerCount);
    }

    public void MaxFollowerIncrease(int amount)
    {
        maxFollowers += amount;
    }
}

public enum FollowerState
{
    Following, // Actively following the player
    Idle,      // Idle and not following the player
    Busy,      // Busy with a task
    Carrying,
    Dead       // Dead/destroyed
}

public enum FollowerType
{
    Any,
    Base,
    Fire,
    Water,
    Poison
}
