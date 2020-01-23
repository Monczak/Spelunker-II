using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Level
{
    public string seed;

    // Level data

    public int width, height;
    public int randomFillPercent;

    public Vector2Int exitSize;
    public int borderSize = 12;

    public LevelTile[,] map, borderedMap;

    public List<List<Coord>> wallRegions;
    public List<List<Coord>> roomRegions;

    public List<Room> rooms;
    public List<Coord> mineralCoordList;

    // Other stuff goes here

    public Vector3Int gridPosition;    

    public LevelConnection[] sideConnections;
    public LevelConnection[] verticalConnections;

    public int depth, verticalDepth;
    public int intensity = 1;

    public Dictionary<HorizontalDirection, int[]> edgeDistances;
    public Dictionary<HorizontalDirection, List<LevelExitPath>> shortestExitPaths;

    public void Preprocess()
    {
        ComputeEdgeDistances();
        GetShortestExitPaths();
    }

    void ComputeEdgeDistances()
    {
        edgeDistances = new Dictionary<HorizontalDirection, int[]>
        {
            { HorizontalDirection.West, new int[height] },
            { HorizontalDirection.North, new int[width] },
            { HorizontalDirection.East, new int[height] },
            { HorizontalDirection.South, new int[width] }
        };

        for (int y = 0; y < height; y++)
        {
            int x;
            for (x = 0; x < width; x++)
                if (map[x, y].type == LevelTileType.Nothing) break;
            edgeDistances[HorizontalDirection.West][y] = x;
        }

        for (int x = 0; x < width; x++)
        {
            int y;
            for (y = height - 1; y > 0; y--)
                if (map[x, y].type == LevelTileType.Nothing) break;
            edgeDistances[HorizontalDirection.North][x] = height - y;
        }

        for (int y = 0; y < height; y++)
        {
            int x;
            for (x = width - 1; x > 0; x--)
                if (map[x, y].type == LevelTileType.Nothing) break;
            edgeDistances[HorizontalDirection.East][y] = width - x;
        }

        for (int x = 0; x < width; x++)
        {
            int y;
            for (y = 0; y < height; y++)
                if (map[x, y].type == LevelTileType.Nothing) break;
            edgeDistances[HorizontalDirection.South][x] = y;
        }
    }

    void GetShortestExitPaths()
    {
        shortestExitPaths = new Dictionary<HorizontalDirection, List<LevelExitPath>>();
        for (int i = 0; i < 4; i++)
        {
            shortestExitPaths[(HorizontalDirection)i] = new List<LevelExitPath>();
            int min = int.MaxValue;
            List<int> indices = new List<int>();

            for (int j = 0; j < edgeDistances[(HorizontalDirection)i].Length; j++)
                if (edgeDistances[(HorizontalDirection)i][j] < min)
                    min = edgeDistances[(HorizontalDirection)i][j];

            for (int j = 0; j < edgeDistances[(HorizontalDirection)i].Length; j++)
                if (edgeDistances[(HorizontalDirection)i][j] == min)
                    indices.Add(j);

            shortestExitPaths[(HorizontalDirection)i].Add(new LevelExitPath
            {
                distance = min,
                coordinates = indices
            });
        }
    }

    public LevelExitPath GetExitPathFor(HorizontalDirection direction)
    {
        try
        {
            return shortestExitPaths[direction][GameManager.Instance.levelGenRng.Next(shortestExitPaths[direction].Count)];
        }
        catch
        {
            return null;
        }
    }

    public void Pregenerate()
    {
        GameManager.Instance.mapGenerator.width = width;
        GameManager.Instance.mapGenerator.height = height;
        GameManager.Instance.mapGenerator.borderSize = borderSize;

        List<LevelTile[,]> mapList = GameManager.Instance.mapGenerator.GenerateMapTiles();
        map = mapList[0];
        borderedMap = mapList[1];

        Preprocess();
    }

    public void Generate()
    {
        ModifyMap();

        GameManager.Instance.mapGenerator.map = map;
        GameManager.Instance.mapGenerator.borderedMap = borderedMap;

        GameManager.Instance.mapGenerator.ProcessMap();

        wallRegions = GameManager.Instance.mapGenerator.wallRegions;
        roomRegions = GameManager.Instance.mapGenerator.roomRegions;
        rooms = GameManager.Instance.mapGenerator.caveRooms;
        mineralCoordList = GameManager.Instance.mapGenerator.mineralCoordList;

        foreach (LevelConnection connection in verticalConnections)
        {
            if (connection == null) continue;

            VerticalLevelExit exit = (VerticalLevelExit)connection.exit;
            Debug.Log($"Processing vertical exit in {gridPosition}");
            exit.Generate(rooms);
        }
    }

    public void Generate(LevelGenerationProperties properties)
    {
        width = properties.width;
        height = properties.height;
        seed = properties.seed;
        randomFillPercent = properties.randomFillPercent;

        GameManager.Instance.mapGenerator.randomFillPercent = properties.randomFillPercent;
        GameManager.Instance.rngSeed = properties.seed;
        GameManager.Instance.UpdateRNG();

        Generate();
    }

    public void Load()
    {
        GameManager.Instance.mapGenerator.GenerateMap(this);
    }

    public void Unload()
    {
        foreach (Transform child in GameManager.Instance.mapGenerator.transform)
        {
            if (child.CompareTag("Chunk") || child.CompareTag("Exit")) GameObject.Destroy(child.gameObject);
        }

        // Free memory of unused stray meshes
        Resources.UnloadUnusedAssets();
    }

    public void Save()
    {
        wallRegions = GameManager.Instance.mapGenerator.GetRegions(LevelTileType.Wall);
        roomRegions = GameManager.Instance.mapGenerator.GetRegions(LevelTileType.Nothing);

        rooms = new List<Room>();
        foreach (List<Coord> roomRegion in roomRegions)
            rooms.Add(new Room(roomRegion, map));

        map = GameManager.Instance.mapGenerator.map;
        borderedMap = GameManager.Instance.mapGenerator.borderedMap;
        wallRegions = GameManager.Instance.mapGenerator.wallRegions;
        roomRegions = GameManager.Instance.mapGenerator.roomRegions;
        rooms = GameManager.Instance.mapGenerator.caveRooms;
        mineralCoordList = GameManager.Instance.mapGenerator.mineralCoordList;
    }

    void ModifyMap()
    {
        //Debug.Log(borderedMap.GetLength(0));
        for (int i = 0; i < sideConnections.Length; i++)
        {
            if (sideConnections[i] == null) continue;

            HorizontalLevelExit exit = (HorizontalLevelExit)sideConnections[i].exit;
            //Debug.Log(string.Format("Pos: {0} | Dir: {1}", new Vector2Int(exit.pos.tileX, exit.pos.tileY), exit.direction));

            for (int x = exit.TopLeftPos.tileX; x < exit.TopLeftPos.tileX + exit.size.x; x++)
            {
                for (int y = exit.TopLeftPos.tileY; y < exit.TopLeftPos.tileY + exit.size.y; y++)
                {
                    borderedMap[x, y] = exit.exitArea[x - exit.TopLeftPos.tileX, y - exit.TopLeftPos.tileY];
                }
            }
        }
    }
}

public class LevelExitPath
{
    public List<int> coordinates;
    public int distance;
}
