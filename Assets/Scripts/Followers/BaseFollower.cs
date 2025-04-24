using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFollower : Follower
{
    void Start()
    {
        followerType = FollowerType.Base;
    }
}
