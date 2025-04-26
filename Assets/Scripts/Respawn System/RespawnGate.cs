using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnGate : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.TriggerEvent(EventNames.SpawnPointUpdate);
            this.gameObject.SetActive(false);
            Debug.Log("Gate Triggered");
        }

    }
}
