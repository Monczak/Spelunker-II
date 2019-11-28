using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class MapChunk : MonoBehaviour
{
    [Header("Mesh Filters and Colliders")]
    public MeshFilter caveMeshFilter;
    public MeshFilter wallMeshFilter;
    public MeshFilter wallColliderMeshFilter;
    public MeshCollider caveCollider;
    public MeshCollider wallCollider;

    [HideInInspector] public Mesh caveMesh;
    [HideInInspector] public Mesh wallMesh;
    [HideInInspector] public Mesh wallColliderMesh;

    [Header("Chunk Properties")]
    public int xSize;
    public int zSize;
    public int mapX;
    public int mapZ;

    public MapGenerator mapGen;
    public MeshGenerator meshGen;

    [HideInInspector] public bool updated = false;

    MeshSubGenerator subGen;
    bool genReady = false;
    bool taskReady = false;
    System.Exception genException;
    Task genTask;

    private void Awake()
    {
        /*caveMesh = gameObject.GetComponentsInChildren<MeshFilter>()[0];
        wallMesh = gameObject.GetComponentsInChildren<MeshFilter>()[1];
        caveCollider = gameObject.GetComponentsInChildren<MeshCollider>()[0];
        wallCollider = gameObject.GetComponentsInChildren<MeshCollider>()[1];*/
    }

    public void Init()
    {
        mapGen = transform.parent.GetComponent<MapGenerator>();
        meshGen = transform.parent.GetComponent<MeshGenerator>();
    }

    private void Update()
    {
        if (taskReady && LevelManager.Instance.chunkGenTasksThisFrame++ < LevelManager.Instance.chunkGenTasksPerFrame)
        {
            genTask.Start();
            taskReady = false;
        }


        if (genReady && LevelManager.Instance.chunksLoadedThisFrame++ < LevelManager.Instance.chunksLoadedPerFrame)
        {
            Debug.Log(gameObject.name + " ready!");

            subGen.SetChunkMesh();

            caveMesh = subGen.readyCaveMesh;
            caveMeshFilter.mesh = caveMesh;
            caveMeshFilter.transform.position = Vector3.up * (subGen.wallHeight / 2 + meshGen.relativeY);
            caveCollider.sharedMesh = caveMesh;

            wallMesh = subGen.readyWallMesh;
            wallMeshFilter.mesh = wallMesh;
            wallMeshFilter.transform.position = Vector3.up * (subGen.wallHeight / 2 + meshGen.relativeY);
            wallColliderMeshFilter.mesh = subGen.readyInvertedWallMesh;
            wallColliderMeshFilter.transform.position = wallMeshFilter.transform.position;

            wallCollider.sharedMesh = wallColliderMeshFilter.mesh;

            GameManager.Instance.meshGenerator.squareGrid = subGen.squareGrid;

            genReady = false;

            if (genTask != null) genTask.Dispose();
            genTask = null;
            subGen.Dispose();
            subGen = null;

            GameManager.Instance.meshGenerator.chunksNotLoaded.Remove(this);
        }

        if (genException != null) throw genException;
    }

    // TODO: Consider integration with C# Job System
    public void Create(LevelTile[,] map, float squareSize, bool regen = false)
    {
        subGen = new MeshSubGenerator()
        {
            wallHeight = GameManager.Instance.meshGenerator.wallHeight,
            caveMeshHeightMap = GameManager.Instance.meshGenerator.caveMeshHeightMap,
            map = map,
            squareSize = squareSize,
            chunkProperties = new int[4] { xSize, zSize, mapX, mapZ }
        };

        if (regen)
        {
            subGen.GenerateMesh();
            genReady = true;
            return;
        }

        genTask = new Task(() =>
        {
            try
            {
                subGen.GenerateMesh();
                genReady = true;
            }
            catch (System.Exception e)
            {
                genException = e;
            }
        });
        taskReady = true;
    }

    private void OnDestroy()
    {
        if (genTask != null)
        {
            genTask.Wait();

            genTask.Dispose();
        }
        
    }

    public void SetMeshes(List<Vector3> caveV, List<int> caveT, List<Vector3> wallV, List<int> wallT)
    {
        caveMesh.SetVertices(caveV);
        caveMesh.SetTriangles(caveT, 0);
        caveMesh.RecalculateNormals();

        caveMeshFilter.mesh = caveMesh;

        wallMesh.SetVertices(wallV);
        wallMesh.SetTriangles(wallT, 0);
        wallMesh.RecalculateNormals();

        wallMeshFilter.mesh = wallMesh;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + new Vector3(xSize / 2, 0, zSize / 2), new Vector3(xSize, 10, zSize));
    }
}
