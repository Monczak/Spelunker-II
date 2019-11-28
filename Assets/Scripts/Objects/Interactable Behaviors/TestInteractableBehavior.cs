using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

public class TestInteractableBehavior : InteractableBehavior
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Test Interactable Behavior - Start");
    }

    public override void OnInteracted()
    {
        Debug.Log("Test Interactable Behavior - Interacted");
    }
}
