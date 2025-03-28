using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public int softCap;
    public int hardCap;

    public float detectionRadius = 5f; // Radius to detect and add followers
    public float removalRadius = 7f; // Radius to remove followers
    public LayerMask followerLayer; // Layer to detect followers

    public GameObject prompt;
    public TextMeshPro counter;

    public List<GameObject> Workers = new List<GameObject>(); // Followers within proximity

    private void OnEnable()
    {
        // Subscribe to events
        EventManager.StartListening(EventNames.FollowerRecalled, OnFollowerRecalled);
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        EventManager.StopListening(EventNames.FollowerRecalled, OnFollowerRecalled);
    }

    private void Start()
    {
        if (counter == null)
        {
            Debug.Log("Counter not assigned");
        }
        else
        {
            UpdateCounter();
        }
    }

    protected virtual void Update()
    {
        DetectAndAssignWorkers();
        CheckSoftCap();
    }

    void CheckSoftCap()
    {
        // Check if the soft cap is reached
        if (Workers.Count >= softCap)
        {
            //Debug.Log("Soft cap reached!");
            OnSoftCapReached();
        }
        else if (Workers.Count < softCap)
        {
            //Debug.Log("Soft cap no longer reached.");
            OnSoftCapNotReached();
        }
    }

    void DetectAndAssignWorkers()
    {
        // Detect followers within the radius
        Collider[] detectedFollowers = Physics.OverlapSphere(transform.position, detectionRadius, followerLayer);

        foreach (GameObject worker in new List<GameObject>(Workers))
        {
            if (worker == null) continue;

            float distance = Vector3.Distance(transform.position, worker.transform.position);
            if (distance > removalRadius)
            {
                Follower followerScript = worker.GetComponent<Follower>();
                if (followerScript != null)
                {
                    followerScript.SetBusy(false);
                    followerScript.SetCarrying(false);
                }
                Workers.Remove(worker);
            }
        }

        foreach (Collider col in detectedFollowers)
        {
            GameObject follower = col.gameObject;
            Follower followerScript = follower.GetComponent<Follower>();

            if (followerScript != null && followerScript.IsIdle() &&
                !Workers.Contains(follower) && Workers.Count < hardCap)
            {
                followerScript.SetBusy(true);
                Workers.Add(follower);
            }
        }

        // Remove followers that are outside the radius
        //Workers.RemoveAll(follower => {
        //    if (follower == null) return true; // Remove if destroyed

        //    float distance = Vector3.Distance(transform.position, follower.transform.position);
        //    if (distance > removalRadius)
        //    {
        //        Follower followerScript = follower.GetComponent<Follower>();
        //        if (followerScript != null)
        //        {
        //            followerScript.SetBusy(false); // Reset the busy state
        //        }
        //        return true; // Remove from Workers list
        //    }
        //    return false; // Keep in Workers list
        //});

        UpdateCounter();
    }

    private void OnFollowerRecalled(object followerObj)
    {
        GameObject follower = (GameObject)followerObj;
        if (Workers.Contains(follower))
        {
            Workers.Remove(follower);

            Follower followerScript = follower.GetComponent<Follower>();
            if (followerScript != null && followerScript.IsCarrying())
            {
                OnSoftCapNotReached();
            }

            Debug.Log(follower.name + " removed from Workers list due to recall.");
        }
    }

    public void Locked()
    {
        if (prompt != null)
        {
            prompt.SetActive(true);
        }
    }

    public void UnLocked()
    {
        if (prompt != null)
        {
            prompt.SetActive(false);
        }
    }

    public void UpdateCounter()
    {
        counter.text = (Workers.Count + " / " + softCap);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, removalRadius);
    }

    protected abstract void OnSoftCapReached();
    protected abstract void OnSoftCapNotReached();
}
