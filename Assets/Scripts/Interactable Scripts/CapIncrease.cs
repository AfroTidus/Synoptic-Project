using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class CapIncrease : CarryInteractable
{
    [Header("Max Follower Change")]
    public int plusMaxFollowers;
    protected override void OnDestinationReached()
    {
        FollowerManager.Instance.MaxFollowerIncrease(plusMaxFollowers);   
    }
}
