using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class VerticalLevelExit : LevelExit
{
    public VerticalDirection direction;

    public GameObject ladder;

    public Vector2Int playerSpawnPos;
    public int minSpawnRadius = 1, maxSpawnRadius = 2;

    public void Generate(List<Room> rooms)
    {
        bool success = false;
        while (!success)
        {
            Room pickedRoom = rooms[GameManager.Instance.levelGenRng.Next() % rooms.Count];

            Coord pickedTile = pickedRoom.innerTiles[GameManager.Instance.levelGenRng.Next() % pickedRoom.innerTiles.Count];
            pos = pickedTile;

            try
            {
                SetPlayerSpawnPos();
            }
            catch
            {
                continue;
            }
            success = true;
        }
        
    }

    public void SetPlayerSpawnPos()
    {
        Coord pickedTile;

        List<Coord> potentialCoords = new List<Coord>();
        for (int x = -maxSpawnRadius; x <= maxSpawnRadius; x++)
            for (int y = -maxSpawnRadius; y <= maxSpawnRadius; y++)
                if ((x <= -minSpawnRadius || x >= minSpawnRadius) && (y <= -minSpawnRadius || y >= minSpawnRadius))
                    if (GameManager.Instance.mapGenerator.borderedMap[pos.tileX + x + GameManager.Instance.mapGenerator.borderSize, pos.tileY + y + GameManager.Instance.mapGenerator.borderSize].type == LevelTileType.Nothing)
                        potentialCoords.Add(new Coord(pos.tileX + x, pos.tileY + y));

        if (potentialCoords.Count == 0)
        {
            Debug.LogError($"No spawning space found for exit at {pos.tileX} {pos.tileY}, regenerating exit");
            throw new Exception();
        }

        pickedTile = potentialCoords[GameManager.Instance.levelGenRng.Next(potentialCoords.Count)];
        playerSpawnPos = new Vector2Int(pickedTile.tileX, pickedTile.tileY);
    }
}

