using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DestructableInteractable : Interactable
{
    protected override void OnSoftCapReached()
    {
        ReleaseWorkers();
        Destroy(gameObject);
    }

    protected override void OnSoftCapNotReached()
    {
 
    }

    private void ReleaseWorkers()
    {
        foreach (GameObject worker in Workers)
        {
            Follower follower = worker?.GetComponent<Follower>();
            if (follower != null)
            {
                follower.SetBusy(false);
            }
        }

        Workers.Clear();
    }
}
