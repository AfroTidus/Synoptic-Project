using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Singleton to ensure an EventManager exists and there is only one created
    private static EventManager _instance;
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EventManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("EventManager");
                    _instance = go.AddComponent<EventManager>();
                }
            }
            return _instance;
        }
    }

    // Dictionary to store events
    private Dictionary<string, Action<object>> eventDictionary;

    private void Awake()
    {
        // Destroy duplicates and initialize dictionary
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            eventDictionary = new Dictionary<string, Action<object>>();
        }
    }

    // Allows a listener to subscribe to an event
    public static void StartListening(string eventName, Action<object> listener)
    {
        if (Instance.eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent += listener;
            Instance.eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    // Allows a listener to unsubscribe from an event
    public static void StopListening(string eventName, Action<object> listener)
    {
        if (Instance == null) return;
        if (Instance.eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent -= listener;
            if (thisEvent == null)
            {
                Instance.eventDictionary.Remove(eventName);
            }
            else
            {
                Instance.eventDictionary[eventName] = thisEvent;
            }
        }
    }

    // Trigger event and notify listeners
    public static void TriggerEvent(string eventName, object parameter = null)
    {
        if (Instance.eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent.Invoke(parameter);
        }
    }
}

public static class EventNames
{
    public const string PlayerEnteredSpawnRadius = "PlayerEnteredSpawnRadius";
    public const string PlayerExitedSpawnRadius = "PlayerExitedSpawnRadius";
    public const string PlayerPressedSpawnKey = "PlayerPressedSpawnKey";

    public const string FollowerStateChanged = "FollowerStateChanged";
    public const string FollowerSpawned = "FollowerSpawned";
    public const string FollowerDeath = "FollowerDeath";
    public const string FollowerRecalled = "FollowerRecalled";

    public const string CarryInteractableApproachedDestination = "CarryInteractableApproachedDestination";
    public const string CarryInteractableReachedDestination = "CarryInteractableReachedDestination";

    public const string FollowerCountChanged = "FollowerCountChanged";
    public const string MaxFollowersReached = "MaxFollowersReached";
    public const string BelowMaxFollowers = "BelowMaxFollowers";
}