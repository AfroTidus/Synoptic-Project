using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public List<Transform> positions = new List<Transform>();
    public GameObject respawnPoint;
    private int currentPosition = 0;
    private bool firstTrigger = true;

    private void OnEnable()
    {
        // Subscribe to events
        EventManager.StartListening(EventNames.SpawnPointUpdate, OnSpawnPointUpdate);
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        EventManager.StopListening(EventNames.SpawnPointUpdate, OnSpawnPointUpdate);
    }

    void OnSpawnPointUpdate(object data = null)
    {
        if (positions.Count == 0)
        {
            Debug.LogWarning("No respawn positions");
            return;
        }
        if (firstTrigger)
        {
            currentPosition = 0;
            firstTrigger = false;
        }
        else
        {
            // Stop at last respawn position
            currentPosition = Mathf.Min(currentPosition + 1, positions.Count - 1);
        }

        respawnPoint.transform.position = positions[currentPosition].position;
        Debug.Log($"Respawn point updated to position {currentPosition}");
    }
}
