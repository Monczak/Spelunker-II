using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GroundChunk : MonoBehaviour
{
    [Header("Mesh Filter and Collider")]
    public MeshFilter groundMesh;
    public MeshCollider groundCollider;

    [Header("Chunk Properties")]
    public int xSize;
    public int zSize;
    public int mapX;
    public int mapZ;

    GroundSubGenerator subGen;
    bool genReady = false;

    private void Awake()
    {
        
    }

    private void Update()
    {
        if (genReady)
        {
            Debug.Log(gameObject.name + " ready!");

            groundMesh.sharedMesh = subGen.groundMesh;
            groundCollider.sharedMesh = subGen.groundMesh;
            genReady = false;
        }
    }

    public void Create()
    {
        subGen = new GroundSubGenerator()
        {
            width = xSize,
            height = zSize,
            mapX = mapX,
            mapZ = mapZ
        };

        subGen.GenerateMesh();
        genReady = true;

        this.enabled = true;
    }
}
