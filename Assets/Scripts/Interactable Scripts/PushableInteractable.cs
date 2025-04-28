using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class PushableInteractable : Interactable
{
    public Transform destination;
    public float moveSpeed = 1f;
    public bool willBeWalkable;
    private bool isMoving = false;
    private bool reachedDestination = false;
    private Vector3 initialPosition;
    private float progress = 0f;
    private NavMeshSurface navMeshSurface;
    private NavMeshModifier navMeshModifier;

    private void Awake()
    {
        navMeshSurface = FindAnyObjectByType<NavMeshSurface>();
        if (this.gameObject.GetComponent<NavMeshModifier>() != null )
        {
            navMeshModifier = GetComponent<NavMeshModifier>();
        }
    }

    private void Start()
    {
        initialPosition = transform.position;
    }

    protected override void Update()
    {
        if (reachedDestination) return;

        base.Update();

        if (isMoving && !reachedDestination)
        {
            MoveTowardsDestination();
            SetWorkerDestinations(this.transform.position);
        }
    }

    protected override void OnSoftCapReached()
    {
        if (!reachedDestination)
        {
            isMoving = true;
        }
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
            ReachedDestination();
        }
    }

    private void ReachedDestination()
    {
        isMoving = false;
        reachedDestination = true;
        counter.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Ground");

        // Release all workers
        foreach (GameObject worker in Workers)
        {
            Follower follower = worker?.GetComponent<Follower>();
            if (follower != null)
            {
                follower.SetBusy(false);
                follower.SetReturning(true);
            }
        }
        Workers.Clear();

        if (navMeshModifier != null)
        {
            navMeshModifier.enabled = false;
        }
        if (willBeWalkable)
        {
            navMeshSurface.BuildNavMesh();
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
