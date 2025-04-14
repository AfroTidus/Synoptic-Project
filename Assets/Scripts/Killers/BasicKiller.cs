using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicKiller : MonoBehaviour
{
    [SerializeField] private LayerMask followerLayer;

    //Use tag to determine if immune or not AKA and not fire tag

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & followerLayer.value) != 0)
        {
            Debug.Log("Test");
            EventManager.TriggerEvent("FollowerDeath", other.gameObject);
        }
    }

}
