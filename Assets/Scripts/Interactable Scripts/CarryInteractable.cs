using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryInteractable : Interactable
{
    private bool isBeingCarried = false;
    private Transform carrier; // Reference to the first worker's transform
    public float carryOffset = 2f; // Vertical offset when being carried

    // Start is called before the first frame update
    protected override void OnSoftCapReached()
    {
        if (Workers.Count > 0 && !isBeingCarried)
        {
            // Start carrying with the first worker
            isBeingCarried = true;
            carrier = Workers[0].transform;

            // Optional: Make the object kinematic so physics don't interfere
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
            // Stop carrying
            isBeingCarried = false;
            carrier = null;

            // Optional: Restore physics
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
        if (isBeingCarried && carrier != null)
        {
            // Follow the carrier with an offset
            transform.position = carrier.position + Vector3.up * carryOffset;
        }
    }
}
