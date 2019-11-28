using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    PlayerController player;

    InteractPrompt interactPrompt;

    // Start is called before the first frame update
    void Start()
    {
        interactPrompt = GetComponentInChildren<InteractPrompt>();
        LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded()
    {
        player = GameManager.Instance.playerController;
        HookIntoPlayerEvents();
    }

    void HookIntoPlayerEvents()
    {
        Debug.Log("HUD - hooking into player events");
        player.OnInteractableChange += Player_OnInteractableChange;
    }

    private void Player_OnInteractableChange(Interactable interactable)
    {
        if (interactable == null) interactPrompt.SetPrompt(false);
        else
        {
            interactPrompt.SetPromptTextFor(interactable);
            interactPrompt.SetPrompt(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
