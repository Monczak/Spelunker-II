using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderAscendBehavior : InteractableBehavior
{
    public override void OnInteracted()
    {
        LevelManager.Instance.LoadLevelFromVerticalExit(LevelManager.Instance.currentLevel.verticalConnections[0]);
    }
}
