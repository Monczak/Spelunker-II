using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelExit
{
    public Coord pos;
    public Vector2Int size;

    public LevelTile[,] exitArea;

    public Coord TopLeftPos
    {
        get
        {
            Vector2Int p = new Vector2Int(pos.tileX, pos.tileY) - new Vector2Int(Mathf.FloorToInt((float)size.x / 2), Mathf.FloorToInt((float)size.y / 2));
            return new Coord(p.x, p.y);
        }
    }
}

public enum HorizontalDirection
{
    West,
    North,
    East,
    South
}

public enum VerticalDirection
{
    Up,
    Down
}
