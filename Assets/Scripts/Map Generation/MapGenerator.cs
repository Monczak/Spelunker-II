using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Unity.Jobs;

public class MapGenerator : MonoBehaviour
{
    [Range(0, 100)]
    public int randomFillPercent;

    public int width, height;

    public int smoothIterations = 5;

    public int borderSize = 10;

    public int wallThresholdSize = 50;
    public int roomThresholdSize = 50;
    public bool removeSmallRooms;

    public LevelTile[,] map, borderedMap;
    public List<List<Coord>> wallRegions;
    public List<List<Coord>> roomRegions;

    private MeshGenerator meshGen;

    public List<Room> caveRooms;
    public List<Coord> mineralCoordList;

    [Header("Mineral Distribution")]
    [Range(0, 100)]
    public int mineralChance;
    [Range(0, 100)]
    public int mineralRoomMinAbundance;
    [Range(0, 100)]
    public int mineralRoomMaxAbundance;
    public Color mineralColor1, mineralColor2;

    private System.Random random;

    private bool isMeshGenerated = false;
    private bool generatingMesh = false;
    private bool finishedGeneratingMesh = false;

    private JobHandle meshGenJob;

    private void Awake()
    {
        meshGen = GetComponent<MeshGenerator>();
        random = GameManager.Instance.rng;
    }

    private void Start()
    {
        //GenerateMap();
    }

    private void Update()
    {
        if (finishedGeneratingMesh)
        {
            Debug.Log("Setting mesh");
            meshGen.GenerateAndSetMesh(width, height, isMeshGenerated);
            finishedGeneratingMesh = false;
        }
    }

    public List<LevelTile[,]> GenerateMapTiles()
    {
        map = new LevelTile[width, height];

        //CaveErosion.Erode(map, new Coord(0, 0), new Coord(width - 1, height - 1));

        RandomFillMap();

        for (int i = 0; i < smoothIterations; i++)
        {
            SmoothMap();
        }
        

        borderedMap = new LevelTile[width + borderSize * 2, height + borderSize * 2];

        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
                    borderedMap[x, y] = map[x - borderSize, y - borderSize];
                else
                    borderedMap[x, y] = new LevelTile(LevelTileType.Wall);
            }
        }

        return new List<LevelTile[,]>() { map, borderedMap };
    }

    public void GenerateMap(Level level, bool regen = false)
    {
        if (!regen)
        {
            map = level.map;
            borderedMap = level.borderedMap;
        }

        if (!generatingMesh)
        {
            UpdateMap(regen);
        }
        
    }

    public void UpdateMap(bool regen = false)
    {
        //Debug.Log("Generating mesh");

        //var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        meshGen.GenerateAndSetMesh(width, height, regen);

        //stopwatch.Stop();
        //Debug.Log(string.Format("Generation finished - took {0} ms", stopwatch.ElapsedMilliseconds));
    }

    public List<List<Coord>> GetRegions(LevelTileType tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y].type == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    public void ProcessMap()
    {
        wallRegions = GetRegions(LevelTileType.Wall);
        roomRegions = GetRegions(LevelTileType.Nothing);

        caveRooms = new List<Room>();

        // Remove small wall regions (pointless rocks, small stalagmites)
        foreach (List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    map[tile.tileX, tile.tileY].type = LevelTileType.Nothing;
                }
            }
        }

        // Remove small rooms
        if (removeSmallRooms)
        {
            foreach (List<Coord> roomRegion in roomRegions)
            {
                if (roomRegion.Count < roomThresholdSize)
                {
                    foreach (Coord tile in roomRegion)
                    {
                        map[tile.tileX, tile.tileY].type = LevelTileType.Wall;
                    }
                }
                else
                {
                    caveRooms.Add(new Room(roomRegion, map));
                }
            }
        }
        else
        {
            foreach (List<Coord> roomRegion in roomRegions)
            {
                caveRooms.Add(new Room(roomRegion, map));
            }
        }

        // Prepare rooms (enemy spawning, ore/mineral abundance etc.)
        PrepareRooms();

        // Determine minerals' locations
        SpawnMinerals();
    }

    void PrepareRooms()
    {
        foreach (Room room in caveRooms)
        {
            room.mineralAbundance = random.Next(mineralRoomMinAbundance, mineralRoomMaxAbundance);
        }
    }

    void SpawnMinerals()
    {
        mineralCoordList = new List<Coord>();
        foreach (Room room in caveRooms)
        {
            foreach (Coord tile in room.edgeTiles)
            {
                int rand = random.Next(mineralRoomMinAbundance, mineralRoomMaxAbundance);
                if (rand < mineralChance && rand < room.mineralAbundance)
                {
                    mineralCoordList.Add(new Coord(tile.tileX, tile.tileY));
                }
            }
        }
    }

    List<Coord> GetRegionTiles(int startX, int startY)
    {
        // Flood fill to find regions
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];

        LevelTileType tileType = map[startX, startY].type;

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));

        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
                    {
                        if (mapFlags[x, y] == 0)
                        {
                            if (map[x, y].type == tileType)
                            {
                                mapFlags[x, y] = 1;
                                queue.Enqueue(new Coord(x, y));
                            }
                        }
                    }
                }
            }
        }

        return tiles;
    }

    public bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    private void RandomFillMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    map[x, y] = new LevelTile(LevelTileType.Wall);
                else
                    map[x, y] = new LevelTile(random.Next(0, 100) < randomFillPercent ? LevelTileType.Wall : LevelTileType.Nothing);
            }
        }
    }

    private void SmoothMap()
    {
        LevelTile[,] newMap = map;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighborWallTiles = GetNeighborWallCount(x, y);
                if (neighborWallTiles > 4)
                    newMap[x, y] = new LevelTile(LevelTileType.Wall);
                else if (neighborWallTiles < 4)
                    newMap[x, y] = new LevelTile(LevelTileType.Nothing);
            }
        }

        map = newMap;
    }

    int GetNeighborWallCount(int gridX, int gridY)
    {
        int count = 0;
        for (int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++)
        {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++)
            {
                if (IsInMapRange(neighborX, neighborY))
                {
                    if (neighborX != gridX || neighborY != gridY)
                        count += map[neighborX, neighborY].type == LevelTileType.Wall ? 1 : 0;
                }
                else
                    count++;
            }
        }

        return count;
    }

    public Vector3 CoordToWorldPoint(Coord coord)
    {
        if (meshGen == null) meshGen = GetComponent<MeshGenerator>();
        return new Vector3(-width / 2 + 0.5f + coord.tileX, meshGen.relativeY + meshGen.wallHeight / 2, -height / 2 + 0.5f + coord.tileY);
    }

    public Coord WorldPointToCoord(Vector3 point)
    {
        return new Coord(Mathf.FloorToInt(point.x) + width / 2, Mathf.FloorToInt(point.z) + height / 2);
    }

    public LevelTile GetTileAt(Coord coord)
    {
        try
        {
            LevelTile tile = map[coord.tileX, coord.tileY];
            return tile;
        }
        catch
        {
            return new LevelTile(LevelTileType.Nothing, true);
        }
        
    }

    public LevelTile GetTileAt(Vector3 point)
    {
        Coord tile = WorldPointToCoord(point);
        return GetTileAt(tile);
    }

    public bool CheckFreeSpace(Coord coord, int radius)
    {
        List<Coord> circleCoords = new List<Coord>();
        for (int x = -radius; x < radius; x++)
        {
            for (int y = -radius; y < radius; y++)
            {
                if (x * x + y * y < radius) circleCoords.Add(new Coord(coord.tileX + x, coord.tileY + y));
            }
        }

        foreach (Coord c in circleCoords)
        {
            if (borderedMap[c.tileX, c.tileY].type == LevelTileType.Wall) return false;
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        /*if (mineralCoordList == null) return;

        foreach (Coord tile in mineralCoordList)
        {
            Gizmos.color = Utils.RandomColorBetween(mineralColor1, mineralColor2, Utils.ColorModel.HSV);
            Gizmos.DrawCube(new Vector3(tile.tileX - width / 2 + 0.5f, transform.position.y, tile.tileY - height/ 2 + 0.5f), Vector3.one * 0.4f);
        }*/
    }

    private void OnDrawGizmosSelected()
    {
        if (meshGen == null) meshGen = GetComponent<MeshGenerator>();
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, meshGen.relativeY + meshGen.wallHeight / 2, transform.position.z), new Vector3(width, meshGen.wallHeight, height));
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, meshGen.relativeY + meshGen.wallHeight / 2, transform.position.z), new Vector3(width + borderSize * 2, meshGen.wallHeight, height + borderSize * 2));
    }
}
