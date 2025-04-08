using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject followerType;
    public GameObject prompt;

    private bool isPlayerInRadius = false;
    public bool canSpawn;

    private void OnEnable()
    {
        // Subscribe to events when the object is enabled
        EventManager.StartListening(EventNames.PlayerEnteredSpawnRadius, OnPlayerEnteredSpawnRadius);
        EventManager.StartListening(EventNames.PlayerExitedSpawnRadius, OnPlayerExitedSpawnRadius);
        EventManager.StartListening(EventNames.PlayerPressedSpawnKey, OnPlayerPressedSpawnKey);

        EventManager.StartListening(EventNames.MaxFollowersReached, OnMaxFollowersReached);
        EventManager.StartListening(EventNames.BelowMaxFollowers, OnBelowMaxFollowers);
    }

    private void OnDisable()
    {
        // Unsubscribe from events when the object is disabled to avoid memory leaks
        EventManager.StopListening(EventNames.PlayerEnteredSpawnRadius, OnPlayerEnteredSpawnRadius);
        EventManager.StopListening(EventNames.PlayerExitedSpawnRadius, OnPlayerExitedSpawnRadius);
        EventManager.StopListening(EventNames.PlayerPressedSpawnKey, OnPlayerPressedSpawnKey);

        EventManager.StopListening(EventNames.MaxFollowersReached, OnMaxFollowersReached);
        EventManager.StopListening(EventNames.BelowMaxFollowers, OnBelowMaxFollowers);
    }

    private void Awake()
    {
        if (followerType == null)
        {
            Debug.LogError("Follower type is not assigned! Disabling spawner.");
            enabled = false;
        }
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

    private void OnMaxFollowersReached(object data)
    {
        canSpawn = false;
    }

    private void OnBelowMaxFollowers(object data)
    {
        canSpawn = true;
    }

    private void OnPlayerPressedSpawnKey(object data)
    {
        // Check if the player is within the radius and pressed the spawn key
        if (isPlayerInRadius && canSpawn)
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
