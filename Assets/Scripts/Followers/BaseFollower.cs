using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFollower : Follower
{
    void Start()
    {
        // Set Type plus code for any type specific interactionss
        followerType = FollowerType.Base;
    }
}
