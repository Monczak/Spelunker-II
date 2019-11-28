using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class HorizontalLevelExit : LevelExit
{
    public HorizontalDirection direction;

    public LevelExitTrigger trigger;

    public HorizontalLevelExit() { }
    public HorizontalLevelExit(Vector2Int _size, HorizontalDirection _dir, Vector2Int _levelBorderedSize, int borderSize, LevelExitPath path)
    {
        //Debug.Log(string.Format("Dir {0}, bordered size {1}", _dir, _levelBorderedSize));

        switch (_dir)
        {
            case HorizontalDirection.West:
                if (path != null)
                    pos = new Coord(_size.x / 2, path.coordinates[GameManager.Instance.rng.Next(path.coordinates.Count - 1)] + borderSize + _size.y / 2);
                else
                    pos = new Coord(_size.x / 2, GameManager.Instance.rng.Next(_size.y / 2, _levelBorderedSize.y - _size.y / 2 - 1));
                break;
            case HorizontalDirection.North:
                _size = new Vector2Int(_size.y, _size.x);
                if (path != null)
                    pos = new Coord(path.coordinates[GameManager.Instance.rng.Next(path.coordinates.Count - 1)] + borderSize + _size.x / 2, _levelBorderedSize.y - _size.y / 2);
                else
                    pos = new Coord(GameManager.Instance.rng.Next(_size.x / 2, _levelBorderedSize.x - _size.x / 2 - 1), _levelBorderedSize.y - _size.y / 2);
                break;
            case HorizontalDirection.East:
                if (path != null)
                    pos = new Coord(_levelBorderedSize.x - _size.x / 2, path.coordinates[GameManager.Instance.rng.Next(path.coordinates.Count - 1)] + borderSize + _size.y / 2);
                else
                    pos = new Coord(_levelBorderedSize.x - _size.x / 2, GameManager.Instance.rng.Next(_size.y / 2, _levelBorderedSize.y - _size.y / 2 - 1));
                break;
            case HorizontalDirection.South:
                _size = new Vector2Int(_size.y, _size.x);
                if (path != null)
                    pos = new Coord(path.coordinates[GameManager.Instance.rng.Next(path.coordinates.Count - 1)] + borderSize + _size.x / 2, _size.y / 2);
                else
                    pos = new Coord(GameManager.Instance.rng.Next(_size.x / 2, _levelBorderedSize.x - size.x / 2 - 1), _size.y / 2);
                break;
            default: break;
        }
        //Debug.Log(string.Format("Pos: {0} {1}", pos.tileX, pos.tileY));
        direction = _dir;

        size = _size;
        Generate(_size);
    }

    public void Generate(Vector2Int size)
    {
        // TODO: Generate a better cave-like exit
        exitArea = new LevelTile[size.x, size.y];

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                exitArea[i, j] = new LevelTile(LevelTileType.Nothing);
            }
        }
    }
}

