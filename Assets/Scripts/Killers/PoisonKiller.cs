using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonKiller : MonoBehaviour
{
    [SerializeField] private LayerMask followerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & followerLayer.value) != 0)
        {
            Follower follower = other.GetComponent<Follower>();

            if (follower != null && !IsPoisonFollower(follower))
            {
                Debug.Log("Killing non-fire follower");
                EventManager.TriggerEvent(EventNames.FollowerDeath, other.gameObject);
            }
        }
    }

    // protect poison followers
    private bool IsPoisonFollower(Follower follower)
    {
        return follower is PoisonFollower;
    }

}
