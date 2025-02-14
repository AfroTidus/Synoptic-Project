using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    //public Transform player;  // Assign the player object in the inspector
    //public float speed = 5f; // Speed of the follower
    //public float stoppingDistance = 2f; // Distance to maintain from the player

    //void Update()
    //{
    //    if (player == null)
    //    {
    //        Debug.LogWarning("Player not assigned to follower script.");
    //        return;
    //    }

    //    // Calculate distance to the player
    //    float distance = Vector3.Distance(transform.position, player.position);

    //    // Move towards the player if outside stopping distance
    //    if (distance > stoppingDistance)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

    //        // Rotate to face the player
    //        Vector3 direction = (player.position - transform.position).normalized;
    //        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    //        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
    //    }
    //}

    public Transform player;  // Assign the player object in the inspector
    public float speed = 5f; // Speed of the follower
    public float stoppingDistance = 2f; // Distance to maintain from the player

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player not assigned to follower script.");
            return;
        }

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
}
