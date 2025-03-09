using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject followerType;
    public GameObject prompt;

    private bool isPlayerInRadius = false;

    private void OnEnable()
    {
        // Subscribe to events when the object is enabled
        EventManager.StartListening("PlayerEnteredSpawnRadius", OnPlayerEnteredSpawnRadius);
        EventManager.StartListening("PlayerExitedSpawnRadius", OnPlayerExitedSpawnRadius);
        EventManager.StartListening("PlayerPressedSpawnKey", OnPlayerPressedSpawnKey);
    }

    private void OnDisable()
    {
        // Unsubscribe from events when the object is disabled to avoid memory leaks
        EventManager.StopListening("PlayerEnteredSpawnRadius", OnPlayerEnteredSpawnRadius);
        EventManager.StopListening("PlayerExitedSpawnRadius", OnPlayerExitedSpawnRadius);
        EventManager.StopListening("PlayerPressedSpawnKey", OnPlayerPressedSpawnKey);
    }

    private void OnPlayerEnteredSpawnRadius(object data)
    {
        // Check if the player entered the spawn radius
        if ((GameObject)data == gameObject)
        {
            isPlayerInRadius = true;
            prompt.SetActive(true);
        }
    }

    private void OnPlayerExitedSpawnRadius(object data)
    {
        // Check if the player exited the spawn radius
        if ((GameObject)data == gameObject)
        {
            isPlayerInRadius = false;
            prompt.SetActive(false);
        }
    }

    private void OnPlayerPressedSpawnKey(object data)
    {
        // Check if the player is within the radius and pressed the spawn key
        if (isPlayerInRadius)
        {
            SpawnFollower();
        }
    }

    private void SpawnFollower()
    {
        if (followerType != null)
        {
            // Instantiate the follower
            Instantiate(followerType, transform.position, transform.rotation);
            Debug.Log("Follower spawned!");
        }
        else
        {
            Debug.LogWarning("Follower type is not assigned!");
        }
    }

    //private void Update()
    //{
    //    // Check if the player is within the radius and presses the spawn key
    //    if (isPlayerInRadius && Input.GetKeyDown(KeyCode.E))
    //    {
    //        SpawnFollower();
    //    }
    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    // Check if the player enters the trigger zone
    //    if (other.CompareTag("Player"))
    //    {
    //        isPlayerInRadius = true;
    //        prompt.SetActive(true);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    // Check if the player exits the trigger zone
    //    if (other.CompareTag("Player"))
    //    {
    //        isPlayerInRadius = false;
    //        prompt.SetActive(false);

    //    }
    //}

    //private void SpawnFollower()
    //{
    //    if (followerType != null)
    //    {
    //        // Instantiate the follower
    //        Instantiate(followerType, transform.position, transform.rotation);
    //        Debug.Log("Follower spawned!");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Follower type is not assigned!");
    //    }
    //}
}
