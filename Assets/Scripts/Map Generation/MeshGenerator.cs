using System;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;
using System.Linq;
using System.Text;

public class MeshGenerator : MonoBehaviour
{
    // Generates a mesh based on level tiles
    // Utilizes the marching squares algorithm
    // Original implementation by Sebastian Lague, optimized and modified for quick mesh regeneration and Spelunker II usage by Monczak

    public SquareGrid squareGrid;

    List<Vector3> vertices;
    List<int> triangles;
    List<Vector3> wallVertices;
    List<int> wallTriangles;

    Vector3[] groundVertices;
    int[] groundTriangles;

    [Header("Mesh Filters")]
    public MeshFilter cave;
    public MeshFilter walls;
    public MeshFilter ground;

    [Header("Mesh Colliders")]
    public MeshCollider wallCollider;

    Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
    List<List<int>> outlines = new List<List<int>>();
    HashSet<int> checkedVertices = new HashSet<int>();  // Much faster contain checks

    [Header("Cave Properties")]
    public float wallHeight = 5;
    public float relativeY;
    public float caveMeshRipple = .3f;
    public float squareSize = 1;

    public float[,] groundHeightMap;
    public float[,] caveMeshHeightMap;

    [Header("Ground Properties")]
    public float groundXScale;
    public float groundYScale;
    public float groundZScale;
    public int quantization;
    public float groundYPosition;
    public float groundHoleDepth;

    [Header("Chunks")]
    public GameObject emptyChunkPrefab;
    public GameObject emptyGroundChunkPrefab;
    public float margin;

    public int chunkSizeX;
    public int chunkSizeZ;
    private int numChunksX, numChunksZ;

    public int groundChunkSizeX;
    public int groundChunkSizeZ;
    private int numGroundChunksX, numGroundChunksZ;

    [HideInInspector] public List<MapChunk> chunks;
    [HideInInspector] public List<GroundChunk> groundChunks;
    [HideInInspector] public List<MapChunk> chunksNotLoaded;

    [Header("Debug")]
    public int gizmoIndex;

    private MapGenerator mapGen;

    [HideInInspector] public Mesh readyCaveMesh;
    [HideInInspector] public Mesh readyWallMesh;

    [HideInInspector] public Vector3 _updatePos1, _updatePos2;

    private bool heightMapGenerated;

    public delegate void OnGroundGeneratedDelegate();
    public event OnGroundGeneratedDelegate OnGroundGenerated;

    public enum PositionInChunk
    {
        Inside,
        LeftEdge,
        TopEdge,
        RightEdge,
        BottomEdge,
        BottomLeftCorner,
        TopLeftCorner,
        TopRightCorner,
        BottomRightCorner
    }

    private void Awake()
    {
        mapGen = GetComponent<MapGenerator>();
    }

    public void GenerateCaveMeshHeightMap(int width, int height)
    {
        caveMeshHeightMap = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                caveMeshHeightMap[x, y] = ((float)GameManager.Instance.rng.NextDouble() * 2 - 1) * caveMeshRipple;
            }
        }

        heightMapGenerated = true;
    }

    public void GenerateMesh(LevelTile[,] map, float squareSize, int[] chunkProperties, bool regen = false)
    {
        
    }

    // The most evil method of this class
    public void InitializeChunks(int w, int h)
    {
        // Destroy all children
        foreach (Transform child in transform)
        {
            if (child.tag == "Chunk")
                Destroy(child.gameObject);
        }

        chunks.Clear();
        groundChunks.Clear();

        // TODO: Optimal chunk division
        int maxChunkX = chunkSizeX, maxChunkZ = chunkSizeZ;

        numChunksX = Mathf.FloorToInt((float)(w + mapGen.borderSize * 2) / maxChunkX) + 1;
        numChunksZ = Mathf.FloorToInt((float)(h + mapGen.borderSize * 2) / maxChunkZ) + 1;

        // Give birth to new children
        for (int x = 0; x < numChunksX; x++)
        {
            for (int z = 0; z < numChunksZ; z++)
            {
                MapChunk chunk = Instantiate(emptyChunkPrefab, new Vector3((transform.position.x - w / 2 - mapGen.borderSize + x * maxChunkX) * margin, transform.position.y, (transform.position.z - h / 2 - mapGen.borderSize + z * maxChunkZ) * margin), Quaternion.identity).GetComponent<MapChunk>();
                chunk.gameObject.name = string.Format("Chunk {0}/{1}", x, z);
                chunk.gameObject.transform.SetParent(transform);
                chunk.Init();

                chunk.xSize = maxChunkX;
                chunk.zSize = maxChunkZ;
                chunk.mapX = x * maxChunkX;
                chunk.mapZ = z * maxChunkZ;
                
                chunks.Add(chunk);
                chunksNotLoaded.Add(chunk);
            }
        }

        // TODO: Optimal ground chunk division
        int maxGroundChunkX = groundChunkSizeX, maxGroundChunkZ = groundChunkSizeZ;

        // Do the same for the ground chunks
        numGroundChunksX = Mathf.FloorToInt((float)(w + mapGen.borderSize * 2) / groundChunkSizeX) + 1;
        numGroundChunksZ = Mathf.FloorToInt((float)(h + mapGen.borderSize * 2) / groundChunkSizeZ) + 1;

        for (int x = 0; x < numGroundChunksX; x++)
        {
            for (int z = 0; z < numGroundChunksZ; z++)
            {
                GroundChunk chunk = Instantiate(emptyGroundChunkPrefab, new Vector3(
                    (transform.position.x - w / 2 - mapGen.borderSize + x * maxGroundChunkX) * margin,
                    groundYPosition,
                    (transform.position.z - h / 2 - mapGen.borderSize + z * maxGroundChunkZ) * margin
                    ), Quaternion.identity).GetComponent<GroundChunk>();
                chunk.gameObject.name = string.Format("Ground Chunk {0}/{1}", x, z);
                chunk.gameObject.transform.SetParent(transform);

                chunk.xSize = Mathf.Min(maxGroundChunkX, w + mapGen.borderSize * 2 - maxGroundChunkX * x);
                chunk.zSize = Mathf.Min(maxGroundChunkZ, h + mapGen.borderSize * 2 - maxGroundChunkZ * z);
                chunk.mapX = x * maxGroundChunkX;
                chunk.mapZ = z * maxGroundChunkZ;

                groundChunks.Add(chunk);
            }
        }
    }

    public int GetChunkIndexAt(Vector2Int pos)
    {
        return ((pos.x + mapGen.width / 2 + mapGen.borderSize) / chunkSizeX) * numChunksZ + (pos.y + mapGen.height / 2 + mapGen.borderSize) / chunkSizeZ;
    }

    public MapChunk GetChunkAt(Vector3 pos)
    {
        Vector2Int roundedPos = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));

        if (roundedPos.x < -mapGen.width / 2 - mapGen.borderSize || roundedPos.x > mapGen.width / 2 + mapGen.borderSize || roundedPos.y < -mapGen.height / 2 - mapGen.borderSize || roundedPos.y > mapGen.height / 2 + mapGen.borderSize)
            throw new OutOfBoundsException();

        return chunks[GetChunkIndexAt(roundedPos)];
    }

    public Vector2Int GetPositionInChunk(Vector3 pos)
    {
        MapChunk currentChunk = GetChunkAt(pos);
        Vector3 relativePos = pos - new Vector3(currentChunk.mapX, 0, currentChunk.mapZ);
        return new Vector2Int(Mathf.FloorToInt(relativePos.x) + (mapGen.width + mapGen.borderSize * 2) / 2, Mathf.FloorToInt(relativePos.z) + (mapGen.height + mapGen.borderSize * 2) / 2);
    }

    public PositionInChunk GetEdgePositionInChunk(Vector3 pos)
    {
        Vector2Int roundedPos = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));

        if (roundedPos.x < -mapGen.width / 2 - mapGen.borderSize || roundedPos.x > mapGen.width / 2 + mapGen.borderSize || roundedPos.y < -mapGen.height / 2 - mapGen.borderSize || roundedPos.y > mapGen.height / 2 + mapGen.borderSize)
            throw new OutOfBoundsException();

        Vector2Int chunkPos = GetPositionInChunk(pos);

        if (chunkPos.x == 0 && chunkPos.y == 0) return PositionInChunk.BottomLeftCorner;
        if (chunkPos.x == chunkSizeX - 1 && chunkPos.y == 0) return PositionInChunk.BottomRightCorner;
        if (chunkPos.x == 0 && chunkPos.y == chunkSizeZ - 1) return PositionInChunk.TopLeftCorner;
        if (chunkPos.x == chunkSizeX - 1 && chunkPos.y == chunkSizeZ - 1) return PositionInChunk.TopRightCorner;

        if (chunkPos.x == 0) return PositionInChunk.LeftEdge;
        if (chunkPos.x == chunkSizeX - 1) return PositionInChunk.RightEdge;
        if (chunkPos.y == 0) return PositionInChunk.BottomEdge;
        if (chunkPos.y == chunkSizeZ - 1) return PositionInChunk.TopEdge;

        return PositionInChunk.Inside;
    }

    public void UpdateChunks(float minChunkX, float minChunkZ, float maxChunkX, float maxChunkZ)
    {
        foreach (MapChunk chunk in chunks) chunk.updated = false;

        for (float x = minChunkX; x <= maxChunkX; x += chunkSizeX)
        {
            for (float z = minChunkZ; z <= maxChunkZ; z += chunkSizeZ)
            {
                MapChunk chunk = GetChunkAt(new Vector3(x, 0, z));
                if (!chunk.updated)
                {
                    chunk.updated = true;

                    chunk.Create(mapGen.borderedMap, squareSize, true);

                    // Spaghetti starts here
                    // Probably no better way to do this
                    // I'm sorry, Stack Overflow

                    PositionInChunk pic = GetEdgePositionInChunk(new Vector3(x, 0, z));
                    switch (pic)
                    {
                        // Corners
                        case PositionInChunk.BottomLeftCorner:
                            {
                                MapChunk c1 = GetChunkAt(new Vector3(x - 1, 0, z - 1));
                                MapChunk c2 = GetChunkAt(new Vector3(x - 1, 0, z));
                                MapChunk c3 = GetChunkAt(new Vector3(x, 0, z - 1));
                                c1.Create(mapGen.borderedMap, squareSize, true);
                                c2.Create(mapGen.borderedMap, squareSize, true);
                                c3.Create(mapGen.borderedMap, squareSize, true);
                            }
                            break;
                        case PositionInChunk.TopLeftCorner:
                            {
                                MapChunk c1 = GetChunkAt(new Vector3(x - 1, 0, z + 1));
                                MapChunk c2 = GetChunkAt(new Vector3(x - 1, 0, z));
                                MapChunk c3 = GetChunkAt(new Vector3(x, 0, z + 1));
                                c1.Create(mapGen.borderedMap, squareSize, true);
                                c2.Create(mapGen.borderedMap, squareSize, true);
                                c3.Create(mapGen.borderedMap, squareSize, true);
                            }
                            break;
                        case PositionInChunk.TopRightCorner:
                            {
                                MapChunk c1 = GetChunkAt(new Vector3(x + 1, 0, z + 1));
                                MapChunk c2 = GetChunkAt(new Vector3(x + 1, 0, z));
                                MapChunk c3 = GetChunkAt(new Vector3(x, 0, z + 1));
                                c1.Create(mapGen.borderedMap, squareSize, true);
                                c2.Create(mapGen.borderedMap, squareSize, true);
                                c3.Create(mapGen.borderedMap, squareSize, true);
                            }
                            break;
                        case PositionInChunk.BottomRightCorner:
                            {
                                MapChunk c1 = GetChunkAt(new Vector3(x + 1, 0, z - 1));
                                MapChunk c2 = GetChunkAt(new Vector3(x + 1, 0, z));
                                MapChunk c3 = GetChunkAt(new Vector3(x, 0, z - 1));
                                c1.Create(mapGen.borderedMap, squareSize, true);
                                c2.Create(mapGen.borderedMap, squareSize, true);
                                c3.Create(mapGen.borderedMap, squareSize, true);
                            }
                            break;

                        // Edges
                        case PositionInChunk.LeftEdge:
                            {
                                MapChunk c1 = GetChunkAt(new Vector3(x - 1, 0, z));
                                c1.Create(mapGen.borderedMap, squareSize, true);
                            }
                            break;
                        case PositionInChunk.TopEdge:
                            {
                                MapChunk c1 = GetChunkAt(new Vector3(x, 0, z + 1));
                                c1.Create(mapGen.borderedMap, squareSize, true);
                            }
                            break;
                        case PositionInChunk.RightEdge:
                            {
                                MapChunk c1 = GetChunkAt(new Vector3(x + 1, 0, z));
                                c1.Create(mapGen.borderedMap, squareSize, true);
                            }
                            break;
                        case PositionInChunk.BottomEdge:
                            {
                                MapChunk c1 = GetChunkAt(new Vector3(x, 0, z - 1));
                                c1.Create(mapGen.borderedMap, squareSize, true);
                            }
                            break;


                        case PositionInChunk.Inside:
                        default:
                            break;
                    }

                    // Spaghetti ends here
                    // Hopefully tasted good
                }

            }
        }
    }

    /// <summary>
    /// Assigns the computed cave mesh to readyCaveMesh.
    /// DEPRECATED - use subgens instead.
    /// </summary>
    /// <param name="chunkProperties">Chunk properties: x size, z size, x offset, y offset.</param>
    public void SetChunkMesh(int[] chunkProperties)
    {
        Mesh mesh = new Mesh();
        readyCaveMesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    /// <summary>
    /// Creates cave chunks, cave and wall meshes and a ground mesh.
    /// DEPRECATED - use subgens instead.
    /// </summary>
    /// <param name="w">Map width.</param>
    /// <param name="h">Map height.</param>
    /// <param name="regen">Regenerate flag. If true, will only update certain chunks.</param>
    public void GenerateAndSetMesh(int w, int h, bool regen = false)
    {
        if (!regen)
        {
            GenerateCaveMeshHeightMap(w + mapGen.borderSize * 2, h + mapGen.borderSize * 2);
            InitializeChunks(w, h);
        }

        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if (!regen)
        {
            foreach (MapChunk chunk in chunks)
                chunk.Create(mapGen.borderedMap, squareSize, regen);
        }
        else
        {
            UpdateChunks(_updatePos1.x, _updatePos1.z, _updatePos2.x, _updatePos2.z);
        }

        if (!regen)
        {
            GenerateGroundHeightMap(w + mapGen.borderSize * 2, h + mapGen.borderSize * 2);
            GenerateGround();
        }
    }

    private void GenerateGround()
    {
        foreach (GroundChunk chunk in groundChunks)
        {
            chunk.Create();
        }
        OnGroundGenerated?.Invoke();
    }

    private void GenerateGroundMesh(int width, int height)
    {
        
    }

    void GenerateGroundHeightMap(int width, int height)
    {
        groundHeightMap = new float[width + 1, height + 1];
        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                groundHeightMap[x, y] = Utils.Quantize(Mathf.PerlinNoise(x * groundXScale, y * groundZScale), quantization) * groundYScale;
            }
        }

        // TODO: Dig holes for descending ladders
        LevelConnection connection;
        if ((connection = LevelManager.Instance.currentLevel.verticalConnections[1]) != null)
        {
            VerticalLevelExit exit = (VerticalLevelExit)connection.exit;
            Debug.Log($"Digging hole at {exit.pos.tileX + mapGen.borderSize}/{exit.pos.tileY + mapGen.borderSize} of depth {groundHoleDepth}");
            groundHeightMap[exit.pos.tileX + mapGen.borderSize, exit.pos.tileY + mapGen.borderSize] -= groundHoleDepth;
        }
    }

    public float GetGroundLevelAt(float x, float z, LayerMask mask)
    {
        Physics.Raycast(new Vector3(x, 100, z), Vector3.down, out RaycastHit hit, 100 + relativeY, mask);
        return hit.point.y;
    }

    public void CreateWallMesh()
    {
        CalculateMeshOutlines();

        wallVertices = new List<Vector3>();
        wallTriangles = new List<int>();

        Mesh wallMesh = new Mesh();
        
        foreach (List<int> outline in outlines)
        {
            for (int i = 0; i < outline.Count - 1; i++)
            {
                int startIndex = wallVertices.Count;
                wallVertices.Add(vertices[outline[i]]);                                 // Top left vertex
                wallVertices.Add(vertices[outline[i + 1]]);                             // Top right vertex
                wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight);       // Bottom left vertex
                wallVertices.Add(vertices[outline[i + 1]] - Vector3.up * wallHeight);   // Bottom right vertex

                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3);

                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 0);
            }
        }

        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();
        wallMesh.RecalculateNormals();
        wallMesh.RecalculateTangents();

        readyWallMesh = wallMesh;
    }

    void TriangulateSquare(Square square)
    {
        // Painstaking very much
        switch (square.configuration)
        {
            case 0:
                break;

            // 1 point
            case 1:
                MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomLeft);
                break;
            case 2:
                MeshFromPoints(square.bottomRight, square.centerBottom, square.centerRight);
                break;
            case 4:
                MeshFromPoints(square.topRight, square.centerRight, square.centerTop);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
                break;

            // 2 points
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                break;
            case 5:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 3 points
            case 7:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 4 points
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);

                // In this case all corner vertices are active - they are surrounded by a wall - cannot be outline
                // So let's not check them
                // Fooling the algorithm into thinking it had already checked them, lol
                checkedVertices.Add(square.topLeft.vertexIndex);
                checkedVertices.Add(square.topRight.vertexIndex);
                checkedVertices.Add(square.bottomRight.vertexIndex);
                checkedVertices.Add(square.bottomLeft.vertexIndex);
                break;

            default:
                // Don't know how this would trigger, but it might
                // Just might
                Debug.LogError("Unknown marching squares configuration!");
                break;
        }
    }

    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        if (points.Length >= 3)
            CreateTriangle(points[0], points[1], points[2]);
        if (points.Length >= 4)
            CreateTriangle(points[0], points[2], points[3]);
        if (points.Length >= 5)
            CreateTriangle(points[0], points[3], points[4]);
        if (points.Length >= 6)
            CreateTriangle(points[0], points[4], points[5]);
    }

    private void AssignVertices(Node[] points)
    {
        foreach (Node point in points)
        {
            if (point.vertexIndex == -1)
            {
                point.vertexIndex = vertices.Count;
                vertices.Add(point.position);
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddTriangleToDictionary(triangle.vertexIndexA, triangle);
        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
        AddTriangleToDictionary(triangle.vertexIndexC, triangle);
    }

    void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
    {
        if (triangleDictionary.ContainsKey(vertexIndexKey))
            triangleDictionary[vertexIndexKey].Add(triangle);
        else
        {
            List<Triangle> triangles = new List<Triangle>
            {
                triangle
            };
            triangleDictionary.Add(vertexIndexKey, triangles);
        }

    }

    void CalculateMeshOutlines()
    {
        checkedVertices.Clear();    // Mr Sebastian Lague, your algorithm was broken. But now, it isn't. Huzzah!

        for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
        {
            if (!checkedVertices.Contains(vertexIndex))
            {
                //Debug.Log(string.Format("Finding outline vertex for {0}", vertexIndex));
                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                if (newOutlineVertex != -1)
                {
                    checkedVertices.Add(vertexIndex);

                    List<int> newOutline = new List<int>
                    {
                        vertexIndex
                    };
                    outlines.Add(newOutline);

                    //Debug.Log(string.Format("Following outline for {0}", vertexIndex));
                    FollowOutline(newOutlineVertex, outlines.Count - 1);
                    outlines[outlines.Count - 1].Add(vertexIndex);
                }
            }
            else
            {
                //Debug.Log(string.Format("Vertex {0} has been checked", vertexIndex));
            }
        }
    }

    private void FollowOutline(int vertexIndex, int outlineIndex)
    {
        //Debug.Log(string.Format("{0}", vertexIndex));
        outlines[outlineIndex].Add(vertexIndex);
        checkedVertices.Add(vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

        if (nextVertexIndex != -1)
            FollowOutline(nextVertexIndex, outlineIndex);
    }

    int GetConnectedOutlineVertex(int vertexIndex)
    {
        List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

        foreach (Triangle t in trianglesContainingVertex)
        {
            for (int i = 0; i < 3; i++)
            {
                int vertexB = t[i];
                if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB))
                    if (IsOutlineEdge(vertexIndex, vertexB))
                        return vertexB;
            }
        }

        return -1;
    }

    bool IsOutlineEdge(int vertexA, int vertexB)
    {
        List<Triangle> trianglesContainingVA = triangleDictionary[vertexA];
        int sharedTriangles = 0;

        foreach (Triangle t in trianglesContainingVA)
        {
            if (t.Contains(vertexB)) sharedTriangles++;
            if (sharedTriangles > 1) break; // Not an outline edge if it shares more than 1 triangle
        }

        return sharedTriangles == 1;
    }

    private void OnDrawGizmosSelected()
    {
        int vertexIndex = gizmoIndex;
        Gizmos.color = Color.magenta;
        Vector3 offset = Vector3.up * (relativeY + wallHeight + .01f);
        Gizmos.DrawCube(vertices[vertexIndex] + offset, Vector3.one * .1f);
        foreach (Triangle t in triangleDictionary[vertexIndex])
        {
            Gizmos.color = IsOutlineEdge(t.vertexIndexA, t.vertexIndexB) ? Color.green : Color.cyan;
            Gizmos.DrawLine(vertices[t.vertexIndexA] + offset, vertices[t.vertexIndexB] + offset);
            Gizmos.color = IsOutlineEdge(t.vertexIndexB, t.vertexIndexC) ? Color.green : Color.cyan;
            Gizmos.DrawLine(vertices[t.vertexIndexB] + offset, vertices[t.vertexIndexC] + offset);
            Gizmos.color = IsOutlineEdge(t.vertexIndexC, t.vertexIndexA) ? Color.green : Color.cyan;
            Gizmos.DrawLine(vertices[t.vertexIndexC] + offset, vertices[t.vertexIndexA] + offset);
        }

        Gizmos.color = Color.red;
        foreach (List<int> l in outlines)
        {
            for (int i = 0; i < l.Count - 1; i++)
            {
                Gizmos.DrawLine(wallVertices[i] + offset - Vector3.one * .005f, wallVertices[i + 1] + offset - Vector3.one * .005f);
            }
        }
    }
}
