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

    public Coord playerSpawnCoord;

    public HorizontalLevelExit() { }
    public HorizontalLevelExit(Vector2Int _size, HorizontalDirection _dir, Vector2Int _levelBorderedSize, int borderSize, LevelExitPath path)
    {
        //Debug.Log(string.Format("Dir {0}, bordered size {1}", _dir, _levelBorderedSize));

        switch (_dir)
        {
            case HorizontalDirection.West:
                if (path != null)
                    pos = new Coord(_size.x / 2 + borderSize - _size.x, path.coordinates[GameManager.Instance.levelGenRng.Next(path.coordinates.Count - 1)] + borderSize + _size.y / 2);
                else
                    pos = new Coord(_size.x / 2 + borderSize - _size.x, GameManager.Instance.levelGenRng.Next(_size.y / 2, _levelBorderedSize.y - _size.y / 2 - 1));
                break;
            case HorizontalDirection.North:
                _size = new Vector2Int(_size.y, _size.x);
                if (path != null)
                    pos = new Coord(path.coordinates[GameManager.Instance.levelGenRng.Next(path.coordinates.Count - 1)] + borderSize + _size.x / 2, _levelBorderedSize.y - _size.y / 2 - borderSize + _size.y);
                else
                    pos = new Coord(GameManager.Instance.levelGenRng.Next(_size.x / 2, _levelBorderedSize.x - _size.x / 2 - 1), _levelBorderedSize.y - _size.y / 2 - borderSize + _size.y);
                break;
            case HorizontalDirection.East:
                if (path != null)
                    pos = new Coord(_levelBorderedSize.x - _size.x / 2 - borderSize + _size.x, path.coordinates[GameManager.Instance.levelGenRng.Next(path.coordinates.Count - 1)] + borderSize + _size.y / 2);
                else
                    pos = new Coord(_levelBorderedSize.x - _size.x / 2 - borderSize + _size.x, GameManager.Instance.levelGenRng.Next(_size.y / 2, _levelBorderedSize.y - _size.y / 2 - 1));
                break;
            case HorizontalDirection.South:
                _size = new Vector2Int(_size.y, _size.x);
                if (path != null)
                    pos = new Coord(path.coordinates[GameManager.Instance.levelGenRng.Next(path.coordinates.Count - 1)] + borderSize + _size.x / 2, _size.y / 2 + borderSize - _size.y);
                else
                    pos = new Coord(GameManager.Instance.levelGenRng.Next(_size.x / 2, _levelBorderedSize.x - size.x / 2 - 1), _size.y / 2 + borderSize - _size.y);
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

        int xSize = size.x > size.y ? size.x : size.y, ySize = size.x > size.y ? size.y : size.x;

        LevelTile[,] tempExitArea;

        CaveErosion.r = 3;

        bool complete = false;
        while (!complete)
        {
            tempExitArea = new LevelTile[xSize, ySize];
            CaveErosion.Erode(tempExitArea, new Coord(0, GameManager.Instance.levelGenRng.Next(ySize - 1)), new Coord(xSize - 1, GameManager.Instance.levelGenRng.Next(ySize - 1)));

            switch (direction)
            {
                case HorizontalDirection.West:
                    for (int i = 0; i < xSize; i++)
                        for (int j = 0; j < ySize; j++)
                            exitArea[i, j] = tempExitArea[xSize - i - 1, j];
                    break;
                case HorizontalDirection.North:
                    for (int i = 0; i < xSize; i++)
                        for (int j = 0; j < ySize; j++)
                            exitArea[j, i] = tempExitArea[i, j];
                    break;
                case HorizontalDirection.East:
                    exitArea = tempExitArea;
                    break;
                case HorizontalDirection.South:
                    for (int i = 0; i < xSize; i++)
                        for (int j = 0; j < ySize; j++)
                            exitArea[ySize - j - 1, xSize - i - 1] = tempExitArea[i, j];
                    break;
            }

            bool found = false;
            switch (direction)
            {
                case HorizontalDirection.West:
                    for (int x = 0; x < size.x && !found; x++)
                        for (int y = 0; y < size.y; y++)
                            if (found = exitArea[x, y].type == LevelTileType.Nothing)
                            {
                                playerSpawnCoord = new Coord(x + TopLeftPos.tileX, y + TopLeftPos.tileY);
                                break;
                            }
                    break;
                case HorizontalDirection.North:
                    for (int y = size.y - 1; y > 0 && !found; y--)
                        for (int x = 0; x < size.x; x++)
                            if (found = exitArea[x, y].type == LevelTileType.Nothing)
                            {
                                playerSpawnCoord = new Coord(x + TopLeftPos.tileX, y + TopLeftPos.tileY);
                                break;
                            }
                    break;
                case HorizontalDirection.East:
                    for (int x = size.x - 1; x > 0 && !found; x--)
                        for (int y = 0; y < size.y; y++)
                            if (found = exitArea[x, y].type == LevelTileType.Nothing)
                            {
                                playerSpawnCoord = new Coord(x + TopLeftPos.tileX - 1, y + TopLeftPos.tileY);
                                break;
                            }
                    break;
                case HorizontalDirection.South:
                    for (int y = 0; y < size.y && !found; y++)
                        for (int x = 0; x < size.x; x++)
                            if (found = exitArea[x, y].type == LevelTileType.Nothing)
                            {
                                playerSpawnCoord = new Coord(x + TopLeftPos.tileX, y + TopLeftPos.tileY);
                                break;
                            }
                    break;
            }

            int airCount = 0;
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    if (exitArea[x, y].type == LevelTileType.Nothing) airCount++;

            if (airCount >= 10) complete = true;
        }

        
    }
}

