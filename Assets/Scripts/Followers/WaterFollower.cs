using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFollower : Follower
{
    void Start()
    {
        // Set Type plus code for any type specific interactions
        followerType = FollowerType.Water;
    }
}
