using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Follower : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;

    [Header("Throw Settings")]
    public float speed = 5f; // Speed of the follower
    public float avoidanceDistance = 1.5f;
    public float throwForce = 15f; // Force applied when thrown
    public float throwOffset = 1.5f; // Offset in front of the player
    public float throwDuration = 1.5f; // Time before follower resumes following

    public FollowerType followerType;

    private Rigidbody rb;
    private bool isIdle = false;
    private bool isBusy = false;
    private bool isCarrying = false;
    private bool isDelivering = false;
    private bool isThrown = false;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;

        rb.isKinematic = true;
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.FollowerDeath, OnDeathEvent);
        EventManager.StartListening(EventNames.FollowerMoveToPosition, MoveToPosition);
        EventManager.TriggerEvent(EventNames.FollowerSpawned, this.gameObject);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.FollowerDeath, OnDeathEvent);
        EventManager.StopListening(EventNames.FollowerMoveToPosition, MoveToPosition);
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player not assigned to follower script.");
            return;
        }

        // If the follower is unavailable player
        if (isIdle || isBusy || isThrown || isDelivering) return;

        agent.SetDestination(player.position);

        // Rotate to face the player
        Vector3 lookDirection = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(lookDirection.x, 0, lookDirection.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
    }

    public void Throw()
    {
        if (player == null || rb == null || isThrown || isCarrying) return;

        Debug.Log(name + " is being thrown!");

        agent.enabled = false;
        rb.isKinematic = false;
        isThrown = true;

        // Reset Rigidbody velocity and angular velocity
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Set position slightly in front of the player before launch
        Vector3 throwPosition = player.position + player.forward * throwOffset;
        transform.position = throwPosition;

        // Launch follower in player facing direction
        Vector3 launchDirection = (player.forward + Vector3.up * 0.2f).normalized;
        rb.AddForce(launchDirection * throwForce, ForceMode.Impulse);

        StartCoroutine(SetIdleAfterThrow());
        Debug.Log(name + " has been thrown!");
    }

    private void MoveToPosition(object positionObj)
    {
        if (isIdle || isBusy || isDead || isThrown || isCarrying || isDelivering) return;

        Vector3 targetPosition = (Vector3)positionObj;

        // Set the follower to busy state while moving to commanded position
        SetBusy(true);

        // Set the NavMeshAgent destination
        agent.SetDestination(targetPosition);
        SetIdle(true);
    }

    private void OnDeathEvent(object followerObj)
    {
        GameObject follower = (GameObject)followerObj;
        if (follower == this.gameObject) // Check if this is the follower that should die
        {
            if (isDead) return;

            SetDead(true);

            Destroy(gameObject, 0.5f);
        }
    }

    private IEnumerator SetIdleAfterThrow()
    {
        // Wait for the throw duration
        yield return new WaitForSeconds(throwDuration);

        // Re-enable the NavMeshAgent and disable physics
        rb.isKinematic = true;
        agent.enabled = true;
        isThrown = false;

        SetIdle(true);
    }

    public void SetIdle(bool idle)
    {
        isIdle = idle;
        if (idle) isBusy = false; // Ensure the follower is not busy if set to idle
        NotifyStateChange();
    }

    public bool IsIdle()
    {
        return isIdle;
    }

    public void SetBusy(bool busy)
    {
        isBusy = busy;
        if (busy) isIdle = false; // Ensure the follower is not idle if set to busy
        NotifyStateChange();
    }

    public bool IsBusy()
    {
        return isBusy;
    }

    public void SetCarrying(bool carrying)
    {
        isCarrying = carrying;
        if (carrying)
        {
            isBusy = false;
            isIdle = false;
        }
        NotifyStateChange();
    }

    public bool IsCarrying()
    {
        return isCarrying;
    }

    public void SetDelivering(bool delivering)
    {
        isDelivering = delivering;
    }

    public void SetDead(bool dead)
    {
        isDead = dead;
        if (dead) { isBusy = false; isIdle = false; }
        NotifyStateChange();
    }

    public bool IsDead()
    {
        return isDead;
    }

    public FollowerType GetFollowerType()
    {
        return followerType;
    }

    private void NotifyStateChange()
    {
        EventManager.TriggerEvent(EventNames.FollowerStateChanged, this.gameObject);
    }

    //public void SetBasic(bool State)
    //{
    //    basic = State;
    //    Debug.Log("Follower is Basic");
    //}

    //public void SetFire(bool State)
    //{
    //    fire = State;
    //    Debug.Log("Follower is Fire");
    //}

    //public void SetType2(bool State)
    //{
    //    type2 = State;
    //    Debug.Log("Follower is Type2");
    //}

    //public void SetType3(bool State)
    //{
    //    type3 = State;
    //    Debug.Log("Follower is Type3");
    //}

}

//// Calculate distance to the player
//float distance = Vector3.Distance(transform.position, player.position);

//// Maintain distance from the player
//if (Mathf.Abs(distance - stoppingDistance) > 0.1f)
//{
//    Vector3 direction = (transform.position - player.position).normalized;
//    Vector3 targetPosition = player.position + direction * stoppingDistance;
//    //transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
//}