using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBehavior : MonoBehaviour
{
    public string promptText;

    private void Awake()
    {
        Interactable interactable = GetComponent<Interactable>();
        interactable.OnInteracted += OnInteracted;
        interactable.promptText = promptText;
    }

    public abstract void OnInteracted();
}
