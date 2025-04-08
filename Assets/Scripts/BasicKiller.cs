using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicKiller : MonoBehaviour
{
    [SerializeField] private LayerMask followerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & followerLayer.value) != 0)
        {
            Debug.Log("Test");
            EventManager.TriggerEvent("FollowerDeath", other.gameObject);
        }
    }

}
