using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelGraph
{
    public Dictionary<Vector3Int, Level> grid;

    public int maxSideDepth = 7;
    public int maxVerticalDepth = 5;
    public float sideConnectionChance = .41f;
    public float verticalConnectionChance = .32f;

    public int maxVerticalConnections = 3;

    public LevelGenerationProperties levelGenerationProperties;

    public bool randomSideExitPlacement = false;

    public List<Level> mainPath;
    public Level finalLevel;

    private readonly Vector3Int[] branchDirs = new Vector3Int[4]
    {
        Vector3Int.left,
        new Vector3Int(0, 0, 1),
        Vector3Int.right,
        new Vector3Int(0, 0, -1)
    };

    private readonly Vector3Int[] verticalDirs = new Vector3Int[2]
    {
        Vector3Int.up,
        Vector3Int.down
    };

    private readonly string[] branchDirNames = new string[4] { "west", "north", "east", "south" };
    private readonly string[] verticalDirNames = new string[2] { "up", "down" };

    public void SetupLevels()
    {
        grid = new Dictionary<Vector3Int, Level>
        {
            {   // Starting level
                new Vector3Int(0, 0, 0),
                new Level()
                {
                    // TODO: Attach region properties here
                    width = 64,
                    height = 64,

                    sideConnections = new LevelConnection[4] { null, null, null, null },
                    verticalConnections = new LevelConnection[2] { null, null },
                    gridPosition = new Vector3Int(0, 0, 0),
                    exitSize = new Vector2Int(10, 4)
                }
            }
        };

        grid[Vector3Int.zero].Pregenerate();

        Vector3Int currentPos = new Vector3Int(0, 0, 0);

        GenerateLayer(currentPos, 0);

    }

    public void GenerateLayer(Vector3Int currentPos, int depth)
    {
        NewBranch(currentPos, depth);
    }

    public void NewBranch(Vector3Int pos, int currentDepth = 0)
    {
        //Debug.Log("Branching from " + pos);
        //Debug.Log("Depth: " + currentDepth);
        if (grid[pos].depth > maxSideDepth - 1 || grid[pos].verticalDepth > maxVerticalDepth - 1)
        {
            //Debug.Log("Depth too high, backing out");
            if (finalLevel == null) finalLevel = grid[pos];
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            //Debug.Log("Going " + branchDirNames[i]);
            if (GameManager.Instance.rng.NextDouble() < sideConnectionChance)
            {
                if (grid[pos].sideConnections[i] == null)
                {
                    bool connectedToExistingLevel = false;
                    if (!grid.ContainsKey(pos + branchDirs[i]))
                    {
                        //Debug.Log("Creating level at" + (pos + branchDirs[i]));

                        // Create a new level
                        grid.Add(pos + branchDirs[i], new Level()
                        {
                            // TODO: Attach region properties here
                            width = 64,
                            height = 64,

                            sideConnections = new LevelConnection[4] { null, null, null, null },
                            verticalConnections = new LevelConnection[2] { null, null },
                            depth = currentDepth + 1,
                            gridPosition = pos + branchDirs[i],
                            exitSize = new Vector2Int(10, 4)
                        });

                        grid[pos + branchDirs[i]].Pregenerate();
                    }
                    else
                    {
                        connectedToExistingLevel = true;
                        //Debug.Log("Level already exists - connecting");
                    }

                    // Connect the newly created level and the one we're working on
                    grid[pos + branchDirs[i]].sideConnections[(i + 2) % 4] = new LevelConnection()
                    {
                        from = grid[pos + branchDirs[i]],
                        to = grid[pos],
                        exit = new HorizontalLevelExit(grid[pos + branchDirs[i]].exitSize,
                            (HorizontalDirection)((i + 2) % 4),
                            new Vector2Int(grid[pos + branchDirs[i]].width + grid[pos + branchDirs[i]].borderSize * 2 - 1, grid[pos + branchDirs[i]].height + grid[pos + branchDirs[i]].borderSize * 2 - 1),
                            grid[pos + branchDirs[i]].borderSize,
                            grid[pos + branchDirs[i]].GetExitPathFor((HorizontalDirection)((i + 2) % 4))),
                        type = LevelConnectionType.SideExit
                    };

                    grid[pos].sideConnections[i] = new LevelConnection()
                    {
                        from = grid[pos],
                        to = grid[pos + branchDirs[i]],
                        exit = new HorizontalLevelExit(grid[pos].exitSize,
                            (HorizontalDirection)i,
                            new Vector2Int(grid[pos].width + grid[pos].borderSize * 2 - 1, grid[pos].height + grid[pos].borderSize * 2 - 1),
                            grid[pos].borderSize,
                            grid[pos].GetExitPathFor((HorizontalDirection)i)),
                        type = LevelConnectionType.SideExit
                    };

                    // Branch further
                    if (!connectedToExistingLevel) NewBranch(pos + branchDirs[i], currentDepth + 1);
                }
                //else Debug.Log("Connection " + branchDirNames[i] + " is taken");
            }
            //else Debug.Log("Won't generate connection (RNG)"); 
        }

        if (GameManager.Instance.rng.NextDouble() < verticalConnectionChance)
        {
            // Down - true, up - false
            bool connectionDirection = GameManager.Instance.rng.NextDouble() < .5f || grid[pos].verticalDepth == 0;
            int cDirection = connectionDirection ? 1 : 0;
            //Debug.Log("Going " + (connectionDirection ? "down" : "up"));

            if (grid[pos].verticalConnections[cDirection] == null)
            {
                bool connectedToExistingLevel = false;
                if (!grid.ContainsKey(pos + verticalDirs[cDirection]))
                {
                    //Debug.Log("Creating level at" + (pos + verticalDirs[cDirection]));

                    // Create a new level
                    grid.Add(pos + verticalDirs[cDirection], new Level()
                    {
                        // TODO: Attach region properties here
                        width = 64,
                        height = 64,

                        sideConnections = new LevelConnection[4] { null, null, null, null },
                        verticalConnections = new LevelConnection[2] { null, null },
                        depth = currentDepth + 1,
                        verticalDepth = grid[pos].verticalDepth + 1,
                        gridPosition = pos + verticalDirs[cDirection],
                        exitSize = new Vector2Int(10, 4)
                    });

                    grid[pos + verticalDirs[cDirection]].Pregenerate();
                }
                else
                {
                    connectedToExistingLevel = true;
                    //Debug.Log("Level already exists - connecting");
                }

                // Connect the newly created level and the one we're working on
                grid[pos + verticalDirs[cDirection]].verticalConnections[1 - cDirection] = new LevelConnection()
                {
                    from = grid[pos + verticalDirs[cDirection]],
                    to = grid[pos],
                    exit = new VerticalLevelExit()
                    {
                        direction = (VerticalDirection)(1 - cDirection)
                    }
                };

                grid[pos].verticalConnections[cDirection] = new LevelConnection()
                {
                    from = grid[pos],
                    to = grid[pos + verticalDirs[cDirection]],
                    exit = new VerticalLevelExit()
                    {
                        direction = (VerticalDirection)cDirection
                    }
                };

                // Create a new layer
                // TODO: Make layer depth variable
                if (!connectedToExistingLevel) GenerateLayer(pos + verticalDirs[cDirection], currentDepth);
            }
            //else Debug.Log("Connection " + verticalDirNames[cDirection] + " is taken");
        }
        //else Debug.Log("Backing out");
    }

    public void ProcessGraph()
    {
        mainPath = new List<Level>();

        Debug.Log($"Final level pos: {finalLevel.gridPosition}");

        foreach (Vector3Int pos in Pathfind(new Vector3Int(0, 0, 0), finalLevel.gridPosition, (s, g) => (g - s).sqrMagnitude))
        {
            mainPath.Add(grid[pos]);
            Debug.Log(pos);
        }

        for (int i = 0; i < mainPath.Count; i++)
        {
            mainPath[i].intensity = i + 1;
        }

        foreach (Level level in mainPath)
            SetLevelIntensities(level);
    }

    void SetLevelIntensities(Level source)
    {
        // Breadth-first search

        Dictionary<Level, bool> visited = new Dictionary<Level, bool>();

        foreach (Level level in grid.Values)
            visited[level] = false;

        Queue<Level> queue = new Queue<Level>();

        queue.Enqueue(source);
        visited[source] = true;

        while (queue.Count > 0)
        {
            Level level = queue.Dequeue();

            List<LevelConnection> connections = new List<LevelConnection>(level.sideConnections.Concat(level.verticalConnections));
            foreach (LevelConnection connection in connections)
            {
                if (connection != null)
                {
                    if (!visited[connection.to] && !mainPath.Contains(connection.to))
                    {
                        queue.Enqueue(connection.to);
                        visited[connection.to] = true;

                        connection.to.intensity = Mathf.Max(level.intensity - 1, 1);
                    }
                }
            }
        }
    }

    public void GenerateLevels()
    {
        SetupLevels();
        //Debug.Log("Generating graph done");

        ProcessGraph();
        //Debug.Log("Processing graph done");

        // Temporary!
        levelGenerationProperties = new LevelGenerationProperties()
        {
            width = 96,
            height = 96,
            randomFillPercent = 49,
            seed = "err0r"
        };

        foreach (Level level in grid.Values) level.Generate(levelGenerationProperties);
    }

    List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3Int> totalPath = new List<Vector3Int> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }

        return totalPath;
    }

    public delegate int AStarHeuristic(Vector3Int source, Vector3Int goal);

    public List<Vector3Int> Pathfind(Vector3Int source, Vector3Int goal, AStarHeuristic Heuristic)
    {
        // Simple implementation of A*

        List<Vector3Int> openSet = new List<Vector3Int> { source };
        List<Vector3Int> closedSet = new List<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        Dictionary<Vector3Int, float> gScores = new Dictionary<Vector3Int, float>();
        Dictionary<Vector3Int, float> fScores = new Dictionary<Vector3Int, float>();

        foreach (Level level in grid.Values)
        {
            gScores[level.gridPosition] = Mathf.Infinity;
            fScores[level.gridPosition] = Mathf.Infinity;
        }
        gScores[source] = 0;
        fScores[source] = Heuristic(source, goal);            

        while (openSet.Count != 0)
        {
            // Find level from the open set which has lowest fScore
            Vector3Int currentNode = openSet[0];
            for (int i = 0; i < openSet.Count; i++)
            {
                if (fScores[currentNode] > fScores[openSet[i]])
                {
                    currentNode = openSet[i];
                }
            }

            if (currentNode == goal)
                return ReconstructPath(cameFrom, currentNode);

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Get levels connected to this level
            List<Vector3Int> neighbors = new List<Vector3Int>();
            foreach (LevelConnection connection in grid[currentNode].sideConnections)
                if (connection != null) neighbors.Add(connection.to.gridPosition);
            foreach (LevelConnection connection in grid[currentNode].verticalConnections)
                if (connection != null) neighbors.Add(connection.to.gridPosition);

            foreach (Vector3Int neighbor in neighbors)
            {
                if (closedSet.Contains(neighbor)) continue;

                float tentativeGScore = gScores[currentNode] + (grid[neighbor].depth - grid[currentNode].depth); // Always amounts to 1, but might make sense once we introduce some sort of warp exit thing
                if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                else if (tentativeGScore >= gScores[neighbor]) continue;

                cameFrom[neighbor] = currentNode;
                gScores[neighbor] = tentativeGScore;
                fScores[neighbor] = gScores[neighbor] + Heuristic(neighbor, goal);
            }
        }
        return null;
    }
}
