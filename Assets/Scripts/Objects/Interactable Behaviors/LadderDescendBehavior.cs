using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderDescendBehavior : InteractableBehavior
{
    public override void OnInteracted()
    {
        Debug.Log("Descending!");
        LevelManager.Instance.LoadLevelFromVerticalExit(LevelManager.Instance.currentLevel.verticalConnections[1]);
    }
}
