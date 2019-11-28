using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelTileType
{
    Nothing,
    Wall
}

public class LevelTile
{
    public LevelTileType type;
    public bool isBorder;

    public void Mine()
    {
        if (!isBorder)
        {
            type = LevelTileType.Nothing;
            OnMined();
        }
    }

    public virtual void OnMined()
    {
#if UNITY_EDITOR
        Debug.Log("Tile mined!");
#endif
    }

    public LevelTile(LevelTileType _type, bool _isBorder = false)
    {
        type = _type;
        isBorder = _isBorder;
    }
}
