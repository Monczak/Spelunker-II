using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DebugConsoleInterpreter;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Important Objects")]
    public GameObject playerPrefab;
    public Camera mainCam;

    [Header("Important Scripts")]
    public MapGenerator mapGenerator;
    public MeshGenerator meshGenerator;
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public CameraFollow cameraFollow;

    [Header("Properties")]
    public string rngSeed;

    public System.Random levelGenRng, genericRng;

    public LayerMask groundMask;

    public KeyCode kUp, kDown, kLeft, kRight;

    public List<GameObject> interactables;

    bool willPlayerSpawnRandomly = true;
    bool willPlayerSpawn = true;
    Vector3 spawnPos;
    Coord spawnCoord;

    private void Awake()
    {
        #region Singleton
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);
        #endregion

        groundMask = LayerMask.GetMask("Ground");
        mainCam = Camera.main;
        cameraFollow = mainCam.GetComponent<CameraFollow>();
        genericRng = new System.Random();
        UpdateRNG();

        // TODO: Load from JSON
        Config.SetupHardcoded();

        AutoCommandRegisterer.RegisterCommands();
    }

    public void UpdateRNG()
    {
        levelGenRng = new System.Random(rngSeed.GetHashCode());
    }

    // Start is called before the first frame update
    void Start()
    {
        meshGenerator.OnGroundGenerated += OnGroundGenerated;
        LevelManager.Instance.GenerateLevels();

        LevelManager.Instance.currentLevel = LevelManager.Instance.graph.grid[new Vector3Int(0, 0, 0)];

        LevelManager.Instance.loadingLevel = true;
        LevelManager.Instance.currentLevel.Load();
        //LevelManager.Instance.GenerateLevelFeatures();
    }

    void OnGroundGenerated()
    {
        willPlayerSpawn = true;
    }

    public void PrepareForPlayerSpawn(Vector2Int pos)
    {
        Debug.Log($"Setting spawn position to {pos}");
        Vector3 truePos = mapGenerator.CoordToWorldPoint(new Coord(pos.x, pos.y)) - Vector3.one * .5f;
        spawnPos = truePos;
        spawnCoord = new Coord(pos.x, pos.y);
        willPlayerSpawnRandomly = false;
        willPlayerSpawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (willPlayerSpawn)
        {
            if (willPlayerSpawnRandomly)
            {
                Debug.Log("Spawning player in random position");
                LevelManager.Instance.RandomSpawnPlayer();
            }
            else
            {
                Debug.Log($"Preparing to spawn player at {spawnPos}");
                LevelManager.Instance.SpawnPlayer(spawnPos, spawnCoord);
            }
            willPlayerSpawn = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (KeyValuePair<Vector3Int, Level> l in LevelManager.Instance.graph.grid)
        {
            Gizmos.color = Color.Lerp(Color.white, Color.black, (float)l.Value.depth / LevelManager.Instance.graph.maxSideDepth);
            Gizmos.DrawCube(l.Key * 5 + Vector3.up * 20f, Vector3.one * 5);


            Gizmos.color = Color.yellow;
            foreach (LevelConnection connection in l.Value.sideConnections)
                if (connection != null) Gizmos.DrawLine(connection.from.gridPosition * 5, connection.to.gridPosition * 5);
            foreach (LevelConnection connection in l.Value.verticalConnections)
                if (connection != null) Gizmos.DrawLine(connection.from.gridPosition * 5, connection.to.gridPosition * 5);

            if (l.Value == LevelManager.Instance.graph.finalLevel)
                Gizmos.color = Color.blue;
            else if (LevelManager.Instance.graph.mainPath.Contains(l.Value))
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.yellow;
            Gizmos.DrawCube(l.Key * 5, Vector3.one * Mathf.Lerp(0, 2, (float)l.Value.intensity / LevelManager.Instance.graph.mainPath.Count));

        }
    }
}
