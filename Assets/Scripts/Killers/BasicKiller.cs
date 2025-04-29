using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicKiller : MonoBehaviour
{
    [SerializeField] private LayerMask followerLayer;

    // kills everything
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & followerLayer.value) != 0)
        {
            Follower follower = other.GetComponent<Follower>();
            if (follower != null && !follower.IsDead())
            {
                EventManager.TriggerEvent("FollowerDeath", other.gameObject);
            }
        }
    }
}
