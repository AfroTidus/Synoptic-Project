using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    private Transform player;  // Assign the player object in the inspector
    public float speed = 5f; // Speed of the follower
    public float stoppingDistance = 3f; // Distance to maintain from the player
    public float throwForce = 15f; // Force applied when thrown
    public float throwOffset = 1.5f; // Offset in front of the player
    public float throwDuration = 2f; // Time before follower resumes following
    private Rigidbody rb;
    private bool isIdle = false;
    private bool isBusy = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
        //SetIdle(true);
    }

    void Start()
    {

    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player not assigned to follower script.");
            return;
        }

        // If the follower is idle or thrown, do not follow the player
        if (isIdle || isBusy) return;

        // Calculate distance to the player
        float distance = Vector3.Distance(transform.position, player.position);

        // Maintain distance from the player
        if (Mathf.Abs(distance - stoppingDistance) > 0.1f)
        {
            Vector3 direction = (transform.position - player.position).normalized;
            Vector3 targetPosition = player.position + direction * stoppingDistance;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }

        // Rotate to face the player
        Vector3 lookDirection = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(lookDirection.x, 0, lookDirection.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
    }

    public void Throw()
    {
        Debug.Log(name + " is being thrown!");

        if (player == null || rb == null) return;

        // Set position slightly in front of the player before launch
        Vector3 throwPosition = player.position + player.forward * throwOffset;
        transform.position = throwPosition;

        // Launch follower in player's forward direction
        Vector3 launchDirection = player.forward;
        rb.AddForce(launchDirection * throwForce, ForceMode.Impulse);

        SetIdle(true);

        Debug.Log(name + " has been thrown!");
    }

    public void SetIdle(bool idle)
    {
        isIdle = idle;
        if (idle) isBusy = false; // Ensure the follower is not busy if set to idle
    }

    public bool IsIdle()
    {
        return isIdle;
    }

    public void SetBusy(bool busy)
    {
        isBusy = busy;
        if (busy) isIdle = false; // Ensure the follower is not idle if set to busy
    }

    public bool IsBusy()
    {
        return isBusy;
    }
}