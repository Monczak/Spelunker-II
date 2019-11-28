using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GroundSubGenerator
{
    public Vector3[] groundVertices;
    public int[] groundTriangles;

    public int width, height;

    public Mesh groundMesh;
    [HideInInspector] public bool groundMeshExists = false;

    public int mapX, mapZ;

    public void Init()
    {
        groundMesh = new Mesh();
    }

    public void GenerateMesh()
    {
        if (groundVertices == null) groundVertices = new Vector3[6 * width * height];
        if (groundTriangles == null) groundTriangles = new int[6 * width * height];

        // This code works fine at up to around 36000 vertices (how far I tested), but then breaks for absolutely no reason for larger sizes
        // (instead of ending up at the right place, upper vertices go to the bottom of the mesh)
        // (there is no way such a thing would happen, where is this flaw coming from?!)

        // UPDATE: It might have to do with VBOs

        for (long i = 0, z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                const int delta = 1;

                groundVertices[i + 0] = new Vector3(x, GameManager.Instance.meshGenerator.groundHeightMap[x + mapX, z + mapZ + 1], z + delta);
                groundVertices[i + 1] = new Vector3(x + delta, GameManager.Instance.meshGenerator.groundHeightMap[x + mapX + 1, z + mapZ], z);
                groundVertices[i + 2] = new Vector3(x, GameManager.Instance.meshGenerator.groundHeightMap[x + mapX, z + mapZ], z);

                groundVertices[i + 3] = new Vector3(x, GameManager.Instance.meshGenerator.groundHeightMap[x + mapX, z + mapZ + 1], z + delta);
                groundVertices[i + 4] = new Vector3(x + delta, GameManager.Instance.meshGenerator.groundHeightMap[x + mapX + 1, z + mapZ + 1], z + delta);
                groundVertices[i + 5] = new Vector3(x + delta, GameManager.Instance.meshGenerator.groundHeightMap[x + mapX + 1, z + mapZ], z);

                i += 6;
            }
        }

        groundTriangles = Enumerable.Range(0, 6 * width * height).ToArray();

        groundMesh = MeshStorage.Instance.GetGroundMeshFor(new Coord(mapX, mapZ));
        groundMesh.Clear();

        groundMesh.vertices = groundVertices;
        groundMesh.triangles = groundTriangles;

        groundMesh.RecalculateBounds();
        groundMesh.RecalculateNormals();
    }
}
