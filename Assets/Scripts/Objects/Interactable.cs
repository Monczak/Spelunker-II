using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius;
    public int priority;

    [HideInInspector]
    public string promptText = null;

    public delegate void OnInteractedDelegate();
    public event OnInteractedDelegate OnInteracted;

    public void Interact(Transform interactor)
    {
        if (radius < 0 || (radius >= 0 && (interactor.position - transform.position).magnitude < radius))
        {
            Debug.Log(string.Format("Interacted with {0}", gameObject.name));

            if (OnInteracted != null) OnInteracted.Invoke();
            else Debug.LogError("Interaction has no associated behavior!");
        }
    }
}
