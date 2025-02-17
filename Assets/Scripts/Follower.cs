using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform player;  // Assign the player object in the inspector
    public float speed = 5f; // Speed of the follower
    public float stoppingDistance = 2f; // Distance to maintain from the player
    public float throwForce = 20f; // Force applied when thrown
    public float throwOffset = 1.5f; // Offset in front of the player
    public float throwDuration = 2f; // Time before follower resumes following
    private Rigidbody rb;
    private bool isThrown = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player not assigned to follower script.");
            return;
        }

        //Alternatively follower goes inactive until a recall button is pressed
        if (isThrown) return;

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

    public void PerformAction()
    {
        Debug.Log(name + " is performing an action!");

        if (player == null || rb == null) return;

        // Set position slightly in front of the player before launch
        Vector3 throwPosition = player.position + player.forward * throwOffset;
        transform.position = throwPosition;

        // Launch follower in player's forward direction
        Vector3 launchDirection = player.forward;
        rb.AddForce(launchDirection * throwForce, ForceMode.Impulse);

        StartCoroutine(ThrowCooldown());

        Debug.Log(name + " has been thrown!");
    }

    private IEnumerator ThrowCooldown()
    {
        isThrown = true;
        yield return new WaitForSeconds(throwDuration);
        isThrown = false;
    }


    //public Transform player;  // Assign the player object in the inspector
    //public float speed = 5f; // Speed of the follower
    //public float stoppingDistance = 2f; // Distance to maintain from the player
    //public float avoidanceRadius = 1f; // Radius to avoid other followers
    //public LayerMask followerLayer; // Layer to detect other followers
    //public float positionThreshold = 0.05f; // Threshold to prevent jittering
    //private Rigidbody rb;

    //void Start()
    //{
    //    rb = GetComponent<Rigidbody>();
    //    rb.interpolation = RigidbodyInterpolation.Interpolate;
    //    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    //}

    //void FixedUpdate()
    //{
    //    if (player == null)
    //    {
    //        Debug.LogWarning("Player not assigned to follower script.");
    //        return;
    //    }

    //    // Calculate distance to the player
    //    float distance = Vector3.Distance(transform.position, player.position);

    //    // Maintain distance from the player only if necessary
    //    if (Mathf.Abs(distance - stoppingDistance) > positionThreshold)
    //    {
    //        Vector3 direction = (transform.position - player.position).normalized;
    //        Vector3 targetPosition = player.position + direction * stoppingDistance;

    //        // Avoid other followers
    //        Collider[] nearbyFollowers = Physics.OverlapSphere(transform.position, avoidanceRadius, followerLayer);
    //        Vector3 avoidanceVector = Vector3.zero;
    //        int avoidCount = 0;

    //        foreach (Collider follower in nearbyFollowers)
    //        {
    //            if (follower.transform != transform)
    //            {
    //                avoidanceVector += (transform.position - follower.transform.position).normalized;
    //                avoidCount++;
    //            }
    //        }

    //        if (avoidCount > 0)
    //        {
    //            avoidanceVector /= avoidCount;
    //            targetPosition += avoidanceVector * 0.5f; // Smooth adjustment to avoid crowding
    //        }

    //        rb.MovePosition(Vector3.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime));
    //    }

    //    // Rotate to face the player smoothly
    //    Vector3 lookDirection = (player.position - transform.position).normalized;
    //    if (lookDirection.magnitude > 0.1f)
    //    {
    //        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(lookDirection.x, 0, lookDirection.z));
    //        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * speed));
    //    }
    //}
}
