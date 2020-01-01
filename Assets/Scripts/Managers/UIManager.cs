using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public FadeoutController fadeoutController;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        LevelManager.Instance.OnLevelLoaded += FadeOnLevelLoaded;
        LevelManager.Instance.OnLevelHorizontalTransition += FadeOnLevelExit;
        LevelManager.Instance.OnLevelVerticalTransition += FadeOnLevelExit;
    }

    void FadeOnLevelExit()
    {
        fadeoutController.SetFade(true);
    }

    void FadeOnLevelLoaded()
    {
        fadeoutController.SetFade(false);
    }
}
