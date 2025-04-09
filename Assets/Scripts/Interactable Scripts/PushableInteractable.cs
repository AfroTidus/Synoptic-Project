using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PushableInteractable : Interactable
{
    public Transform destination;
    public float moveSpeed = 1f;
    private bool isMoving = false;
    private Vector3 initialPosition;
    private float progress = 0f;

    private void Start()
    {
        initialPosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();

        if (isMoving)
        {
            MoveTowardsDestination();
            SetWorkerDestinations(this.transform.position);
        }
    }

    protected override void OnSoftCapReached()
    {
        isMoving = true;
    }

    protected override void OnSoftCapNotReached()
    {
        isMoving = false;
    }

    private void MoveTowardsDestination()
    {
        if (destination == null) return;

        // Calculate movement based on number of workers (more workers = faster movement)
        float speedMultiplier = Mathf.Clamp((float)Workers.Count / softCap, 1f, 2f);
        progress += Time.deltaTime * moveSpeed * speedMultiplier;
        progress = Mathf.Clamp01(progress);

        // Move the object
        Vector3 targetPosition = Vector3.Lerp(initialPosition, destination.position, progress);
        targetPosition.y = transform.position.y; // Maintain original Y level
        transform.position = targetPosition;

        if (progress >= 1f)
        {
            isMoving = false;
        }
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
        }
    }
}
