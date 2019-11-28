using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChunkHighlighter : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public MeshGenerator meshGenerator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        MapChunk chunk = meshGenerator.GetChunkAt(transform.position);
        Debug.Log(meshGenerator.GetEdgePositionInChunk(transform.position));

        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(transform.position + Vector3.up * 10, Vector3.one);
        Gizmos.DrawWireCube(chunk.transform.position + new Vector3(chunk.xSize / 2, 0, chunk.zSize / 2), new Vector3(chunk.xSize, 0, chunk.zSize));
    }
}
