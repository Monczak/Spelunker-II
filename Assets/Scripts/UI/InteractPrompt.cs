using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractPrompt : MonoBehaviour
{
    public TMP_Text promptText;
    
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetPromptTextFor(Interactable interactable)
    {
        if (interactable.promptText == null || interactable.promptText == "")
            promptText.text = $"Interact with {interactable.gameObject.name}";
        else
            promptText.text = $"{interactable.promptText}";
    }

    public void SetPrompt(bool enabled)
    {
        if (animator != null) animator.SetBool("Visible", enabled);
    }
}
