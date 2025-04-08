using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableInteractable : Interactable
{
    protected override void OnSoftCapReached()
    {
        Destroy(gameObject);
    }

    protected override void OnSoftCapNotReached()
    {
 
    }
}
