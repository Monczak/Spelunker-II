using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshStorage : MonoBehaviour
{
    public static MeshStorage Instance;

    public Dictionary<Coord, Mesh> caveMeshes;
    public Dictionary<Coord, Mesh> wallMeshes;
    public Dictionary<Coord, Mesh> invertedWallMeshes;
    public Dictionary<Coord, Mesh> groundMeshes;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        if (Instance != this)
            Destroy(gameObject);
        #endregion

        caveMeshes = new Dictionary<Coord, Mesh>();
        wallMeshes = new Dictionary<Coord, Mesh>();
        invertedWallMeshes = new Dictionary<Coord, Mesh>();
        groundMeshes = new Dictionary<Coord, Mesh>();
    }

    public Mesh GetCaveMeshFor(Coord coord)
    {
        if (caveMeshes.ContainsKey(coord)) return caveMeshes[coord];
        else caveMeshes.Add(coord, new Mesh());
        //Debug.Log($"Allocating new cave mesh for {coord.tileX} {coord.tileY}");
        return caveMeshes[coord];
    }

    public Mesh GetWallMeshFor(Coord coord)
    {
        if (wallMeshes.ContainsKey(coord)) return wallMeshes[coord];
        else wallMeshes.Add(coord, new Mesh());
        //Debug.Log($"Allocating new wall mesh for {coord.tileX} {coord.tileY}");
        return wallMeshes[coord];
    }

    public Mesh GetInvertedWallMeshFor(Coord coord)
    {
        if (invertedWallMeshes.ContainsKey(coord)) return invertedWallMeshes[coord];
        else invertedWallMeshes.Add(coord, new Mesh());
        //Debug.Log($"Allocating new inverted wall mesh for {coord.tileX} {coord.tileY}");
        return invertedWallMeshes[coord];
    }

    public Mesh GetGroundMeshFor(Coord coord)
    {
        if (groundMeshes.ContainsKey(coord)) return groundMeshes[coord];
        else groundMeshes.Add(coord, new Mesh());
        //Debug.Log($"Allocating new ground mesh for {coord.tileX} {coord.tileY}");
        return groundMeshes[coord];
    }
}
