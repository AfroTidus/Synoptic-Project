using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarryInteractable : Interactable
{
    private bool isBeingCarried = false;
    private List<Transform> carriers = new List<Transform>();
    public float carryOffset = 2f; // Vertical offset when being carried

    [Header("Destination Settings")]
    public Transform destination;
    public float destinationTriggerRadius = 5f;
    public float destinationReachRadius = 1f;
    private bool movingToDestination = false;

    // Start is called before the first frame update
    protected override void OnSoftCapReached()
    {
        if (Workers.Count > 0 && !isBeingCarried)
        {
            isBeingCarried = true;
            carriers.Clear();

            // Set the carrier follower to carrying state
            foreach (GameObject worker in Workers)
            {
                Follower follower = worker.GetComponent<Follower>();
                if (follower != null)
                {
                    follower.SetCarrying(true);
                    carriers.Add(worker.transform);
                }
            }

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }

    protected override void OnSoftCapNotReached()
    {
        if (isBeingCarried)
        {
            // Reset the carrier follower's state
            foreach (GameObject worker in Workers)
            {
                Follower follower = worker.GetComponent<Follower>();
                if (follower != null)
                {
                    follower.SetCarrying(false);
                }
            }

            isBeingCarried = false;
            carriers.Clear();

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (isBeingCarried)
        {
            UpdateCarryPosition();

            if (destination != null)
            {
                float distanceToDest = Vector3.Distance(transform.position, destination.position);

                if (!movingToDestination && distanceToDest <= destinationTriggerRadius)
                {
                    movingToDestination = true;
                    SetWorkerDestinations(destination.position);
                }
                else if (movingToDestination && distanceToDest <= destinationReachRadius)
                {
                    //Logic before destruction
                    ReleaseWorkers();
                    Destroy(gameObject);
                }
            }
        }
    }

    private void UpdateCarryPosition()
    {
        // Calculate average position of all carriers
        Vector3 averagePosition = Vector3.zero;
        foreach (Transform carrier in carriers)
        {
            if (carrier != null)
            {
                averagePosition += carrier.position;
            }
        }
        averagePosition /= carriers.Count;

        // Move object to average position with offset
        transform.position = averagePosition + Vector3.up * carryOffset;
    }

    private void SetWorkerDestinations(Vector3 targetPos)
    {
        foreach (GameObject worker in Workers)
        {
            Follower follower = worker?.GetComponent<Follower>();
            NavMeshAgent agent = worker?.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.SetDestination(targetPos);
            }
            follower.SetDelivering(true);
        }
    }

    private void ReleaseWorkers()
    {
        foreach (GameObject worker in Workers)
        {
            Follower follower = worker?.GetComponent<Follower>();
            if (follower != null)
            {
                follower.SetDelivering(false);
                follower.SetCarrying(false);
                follower.SetBusy(false);
            }
        }

        Workers.Clear();
        carriers.Clear();
        isBeingCarried = false;
        movingToDestination = false;
    }
}
