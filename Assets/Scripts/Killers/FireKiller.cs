using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireKiller : MonoBehaviour
{
    [SerializeField] private LayerMask followerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & followerLayer.value) != 0)
        {
            Follower follower = other.GetComponent<Follower>();

            if (follower != null && !IsFireFollower(follower))
            {
                Debug.Log("Killing non-fire follower");
                EventManager.TriggerEvent(EventNames.FollowerDeath, other.gameObject);
            }
        }
    }

    private bool IsFireFollower(Follower follower)
    {
        return follower is FireFollower;
    }

}
