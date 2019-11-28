using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshSubGenerator : IDisposable
{
    public LevelTile[,] map;
    public float squareSize;
    public int[] chunkProperties;

    public SquareGrid squareGrid;
    Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
    List<List<int>> outlines = new List<List<int>>();
    HashSet<int> checkedVertices = new HashSet<int>();  // Much faster contain checks

    List<Vector3> vertices;
    List<int> triangles;
    List<Vector3> wallVertices;
    List<int> wallTriangles;

    public float wallHeight = 5;

    public float[,] caveMeshHeightMap;

    [HideInInspector] public Mesh readyCaveMesh;
    [HideInInspector] public Mesh readyWallMesh;
    [HideInInspector] public Mesh readyInvertedWallMesh;

    public void Init()
    {
        readyCaveMesh = new Mesh();
        readyWallMesh = new Mesh();
    }

    public void GenerateMesh()
    {
        outlines.Clear();
        checkedVertices.Clear();
        triangleDictionary.Clear();

        squareGrid = new SquareGrid(map, squareSize, caveMeshHeightMap);

        if (vertices == null) vertices = new List<Vector3>();
        else vertices.Clear();

        if (triangles == null) triangles = new List<int>();
        else triangles.Clear();

        for (int x = 0; x < chunkProperties[0]; x++)
        {
            for (int y = 0; y < chunkProperties[1]; y++)
            {
                if (x + chunkProperties[2] < squareGrid.squares.GetLength(0) && y + chunkProperties[3] < squareGrid.squares.GetLength(1))
                    TriangulateSquare(squareGrid.squares[x + chunkProperties[2], y + chunkProperties[3]]);
            }
        }

        CreateWallMesh();
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

    void AssignVertices(Node[] points)
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

    public void CreateWallMesh()
    {
        CalculateMeshOutlines();

        wallVertices = new List<Vector3>();
        wallTriangles = new List<int>();

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

    public void SetChunkMesh()
    {
        readyCaveMesh = MeshStorage.Instance.GetCaveMeshFor(new Coord(chunkProperties[2], chunkProperties[3]));
        readyWallMesh = MeshStorage.Instance.GetWallMeshFor(new Coord(chunkProperties[2], chunkProperties[3]));
        readyInvertedWallMesh = MeshStorage.Instance.GetInvertedWallMeshFor(new Coord(chunkProperties[2], chunkProperties[3]));

        readyCaveMesh.Clear();
        readyWallMesh.Clear();
        readyInvertedWallMesh.Clear();

        readyCaveMesh.SetVertices(vertices);
        readyCaveMesh.SetTriangles(triangles, 0);
        readyCaveMesh.RecalculateNormals();

        readyWallMesh.SetVertices(wallVertices);
        readyWallMesh.SetTriangles(wallTriangles, 0);
        readyWallMesh.RecalculateNormals();

        readyInvertedWallMesh.SetVertices(wallVertices);
        readyInvertedWallMesh.SetTriangles(wallTriangles, 0);
        readyInvertedWallMesh.SetIndices(readyWallMesh.GetIndices(0).Concat(readyWallMesh.GetIndices(0).Reverse()).ToArray(), MeshTopology.Triangles, 0);
        readyInvertedWallMesh.RecalculateNormals();
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                map = null;
                squareGrid = null;
                vertices = null;
                triangles = null;
                wallVertices = null;
                wallTriangles = null;
                triangleDictionary = null;
                outlines = null;
                checkedVertices = null;
            }

            disposedValue = true;
        }
    }

    ~MeshSubGenerator()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(false);
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        GC.SuppressFinalize(this);
    }
    #endregion
}
