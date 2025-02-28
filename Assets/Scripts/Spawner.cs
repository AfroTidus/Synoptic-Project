using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject followerType;

    private bool isPlayerInRadius = false;

    private void Update()
    {
        // Check if the player is within the radius and presses the spawn key
        if (isPlayerInRadius && Input.GetKeyDown(KeyCode.E))
        {
            SpawnFollower();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInRadius = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player exits the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInRadius = false;
        }
    }

    private void SpawnFollower()
    {
        if (followerType != null)
        {
            // Instantiate the follower at the spawner's position
            Instantiate(followerType, transform.position, transform.rotation);
            Debug.Log("Follower spawned!");
        }
        else
        {
            Debug.LogWarning("Follower type is not assigned!");
        }
    }
}
