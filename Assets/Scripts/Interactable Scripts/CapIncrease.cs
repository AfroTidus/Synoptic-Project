using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.EventSystems;

public class CapIncrease : CarryInteractable
{
    // Increase max followers player can have

    [Header("Max Follower Change")]
    public int plusMaxFollowers;
    protected override void OnDestinationReached()
    {
        followerManager.MaxFollowerIncrease(plusMaxFollowers);
    }
}
