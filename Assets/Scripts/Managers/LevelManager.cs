using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public Level currentLevel;
    public LevelGraph graph;

    public LevelExitTrigger sideExitTrigger;
    public GameObject ladderUp, ladderDown;

    public Vector3 downLadderPositioning, downLadderEuler;

    public bool loadingLevel;

    public int chunksLoadedPerFrame = 1;
    public int chunksLoadedThisFrame = 0;

    public int chunkGenTasksPerFrame = 1;
    public int chunkGenTasksThisFrame = 0;

    public delegate void OnLevelLoadedDelegate();
    public event OnLevelLoadedDelegate OnLevelLoaded;

    public delegate void OnLevelVerticalTransitionDelegate();
    public event OnLevelVerticalTransitionDelegate OnLevelVerticalTransition;

    public delegate void OnLevelHorizontalTransitionDelegate();
    public event OnLevelHorizontalTransitionDelegate OnLevelHorizontalTransition;

    LevelConnection lastLevelConnection;

    float scheduledLoadTime;
    bool scheduledLoadType;
    bool scheduledLoad = false;

    private void Awake()
    {
        #region Singleton
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);
        #endregion


    }

    private void Update()
    {
        if (scheduledLoad && Time.time > scheduledLoadTime)
        {
            if (scheduledLoadType) _LoadLevelFromHorizontalExit();
            else _LoadLevelFromVerticalExit();
            scheduledLoad = false;
        }
    }

    private void LateUpdate()
    {
        chunksLoadedThisFrame = 0;
        chunkGenTasksThisFrame = 0;

        if (GameManager.Instance.meshGenerator.chunksNotLoaded.Count == 0 && loadingLevel)
        {
            Debug.Log("Level loaded!");
            OnLevelLoaded?.Invoke();
            loadingLevel = false;
            GenerateLevelFeatures();
        }
    }

    public void Unload()
    {
        foreach (LevelConnection connection in currentLevel.sideConnections)
        {
            HorizontalLevelExit exit = (HorizontalLevelExit)connection.exit;
            exit.trigger.OnPlayerEnterTrigger -= LoadLevelFromHorizontalExit;
        }
    }

    public void RandomSpawnPlayer()
    {
        List<Room> rooms = currentLevel.rooms;

        if (rooms.Count == 0)
        {
            Debug.LogError("Cannot spawn player - no room!");
            return;
        }

        // Pick a random room
        Room spawnRoom = rooms[GameManager.Instance.rng.Next() % rooms.Count];

        // Pick a random Coord where the player will appear
        // Will need to replace this with the entrance's position
        // Which is going to be set in the same way when creating a level
        Coord spawnCoord = spawnRoom.innerTiles[GameManager.Instance.rng.Next() % spawnRoom.innerTiles.Count];

        Debug.Log(string.Format("Spawn pos X: {0} Y: {1}", spawnCoord.tileX, spawnCoord.tileY));

        Vector3 spawnPos = GameManager.Instance.mapGenerator.CoordToWorldPoint(spawnCoord);
        SpawnPlayer(spawnPos, spawnCoord);
    }

    public void SpawnPlayer(Vector3 spawnPos, Coord spawnCoord)
    {
        GameManager.Instance.playerController = Instantiate(GameManager.Instance.playerPrefab, 
            new Vector3(spawnPos.x, 
            GameManager.Instance.meshGenerator.groundHeightMap[Mathf.Clamp(spawnCoord.tileX, 0, currentLevel.width), Mathf.Clamp(spawnCoord.tileY, 0, currentLevel.height)] + GameManager.Instance.meshGenerator.groundYPosition, spawnPos.z), 
            Quaternion.identity)
            .GetComponent<PlayerController>();
        GameManager.Instance.playerController.transform.Translate(Vector3.up * GameManager.Instance.playerController.GetComponent<Collider>().bounds.size.y / 2);

        Debug.Log($"Spawning player at {GameManager.Instance.playerController.transform.position}");

        GameManager.Instance.cameraFollow.objectToFollow = GameManager.Instance.playerController.transform;
    }

    public void GenerateLevels()
    {
        graph = new LevelGraph();
        graph.GenerateLevels();
    }

    public void GenerateLevelFeatures()
    {
        foreach (LevelConnection connection in currentLevel.sideConnections)
        {
            if (connection == null) continue;

            HorizontalLevelExit exit = (HorizontalLevelExit)connection.exit;
            Vector2Int pos = new Vector2Int(exit.pos.tileX, exit.pos.tileY) - Vector2Int.one * currentLevel.borderSize;
            exit.trigger = Instantiate(sideExitTrigger, GameManager.Instance.mapGenerator.CoordToWorldPoint(new Coord(pos.x, pos.y)) - Vector3.one * .5f, Quaternion.identity);
            exit.trigger.transform.localScale = new Vector3(exit.size.x, GameManager.Instance.meshGenerator.wallHeight, exit.size.y);

            switch (exit.direction)
            {
                case HorizontalDirection.West:
                    exit.trigger.transform.Translate(Vector3.left * 4);
                    break;
                case HorizontalDirection.North:
                    exit.trigger.transform.Translate(Vector3.forward * 4);
                    break;
                case HorizontalDirection.East:
                    exit.trigger.transform.Translate(Vector3.right * 4);
                    break;
                case HorizontalDirection.South:
                    exit.trigger.transform.Translate(Vector3.back * 4);
                    break;
            }

            exit.trigger.connection = connection;
            exit.trigger.OnPlayerEnterTrigger += LoadLevelFromHorizontalExit;

            exit.trigger.transform.parent = GameManager.Instance.mapGenerator.transform;
        }

        foreach (LevelConnection connection in currentLevel.verticalConnections)
        {
            if (connection == null) continue;

            VerticalLevelExit exit = (VerticalLevelExit)connection.exit;
            Vector2Int pos = new Vector2Int(exit.pos.tileX, exit.pos.tileY);

            Debug.Log($"Vertical exit at {exit.pos.tileX}/{exit.pos.tileY} to {connection.to.gridPosition}");
            
            if (exit.direction == VerticalDirection.Up)
            {
                exit.ladder = Instantiate(ladderUp, GameManager.Instance.mapGenerator.CoordToWorldPoint(new Coord(pos.x, pos.y)) + Vector3.up * -6.5f, Quaternion.identity);
                exit.ladder.transform.parent = GameManager.Instance.mapGenerator.transform;
            }
            else
            {
                exit.ladder = Instantiate(ladderDown, GameManager.Instance.mapGenerator.CoordToWorldPoint(new Coord(pos.x, pos.y)) + downLadderPositioning, Quaternion.Euler(downLadderEuler));
                exit.ladder.transform.parent = GameManager.Instance.mapGenerator.transform;
            }
        }

    }

    public void DestroyLevelFeatures()
    {
        foreach (Transform obj in GameManager.Instance.mapGenerator.transform)
        {
            if (obj.gameObject.CompareTag("Level Feature"))
                GameObject.Destroy(obj.gameObject);
        }
    }

    public void LoadLevelFromVerticalExit(LevelConnection connection)
    {
        if (!loadingLevel)
        {
            OnLevelVerticalTransition?.Invoke();

            lastLevelConnection = connection;

            scheduledLoad = true;
            scheduledLoadType = false;
            scheduledLoadTime = Time.time + UIManager.Instance.fadeoutController.duration;
        }
    }

    void _LoadLevelFromVerticalExit()
    {
        VerticalLevelExit exit = (VerticalLevelExit)lastLevelConnection.exit;

        loadingLevel = true;

        Destroy(GameManager.Instance.playerController.digboxController.gameObject);
        Destroy(GameManager.Instance.playerController.gameObject);

        DestroyLevelFeatures();

        currentLevel.Save();
        currentLevel.Unload();

        currentLevel = lastLevelConnection.to;
        currentLevel.Load();
        //GenerateLevelFeatures();

        VerticalLevelExit oppositeExit = (VerticalLevelExit)currentLevel.verticalConnections[(int)(1 - exit.direction)].exit;

        Vector2Int pos = oppositeExit.playerSpawnPos;

        GameManager.Instance.PrepareForPlayerSpawn(pos);
        //StartCoroutine(DelayedLoadFinish());
    }

    private void LoadLevelFromHorizontalExit(LevelConnection connection)
    {
        if (!loadingLevel)
        {
            OnLevelHorizontalTransition?.Invoke();

            lastLevelConnection = connection;

            scheduledLoad = true;
            scheduledLoadType = true;
            scheduledLoadTime = Time.time + UIManager.Instance.fadeoutController.duration;
        }

    }

    void _LoadLevelFromHorizontalExit()
    {
        HorizontalLevelExit exit = (HorizontalLevelExit)lastLevelConnection.exit;
        exit.trigger.OnPlayerEnterTrigger -= LoadLevelFromHorizontalExit;

        loadingLevel = true;
        //Debug.Log(string.Format("Entered trigger to {0}", connection.to.gridPosition));

        //GameManager.Instance.playerController.UnloadInputEvents();

        Destroy(GameManager.Instance.playerController.digboxController.gameObject);
        Destroy(GameManager.Instance.playerController.gameObject);

        DestroyLevelFeatures();

        currentLevel.Save();
        currentLevel.Unload();

        currentLevel = lastLevelConnection.to;
        currentLevel.Load();
        //GenerateLevelFeatures();

        HorizontalLevelExit oppositeExit = (HorizontalLevelExit)currentLevel.sideConnections[((int)((HorizontalLevelExit)lastLevelConnection.exit).direction + 2) % 4].exit;

        Vector2Int pos = new Vector2Int(oppositeExit.pos.tileX, oppositeExit.pos.tileY) - Vector2Int.one * currentLevel.borderSize;

        switch (oppositeExit.direction)
        {
            case HorizontalDirection.West:
                pos += Vector2Int.right * (oppositeExit.size.x / 2 - 1);
                break;
            case HorizontalDirection.North:
                pos += Vector2Int.down * (oppositeExit.size.y / 2 - 1);
                break;
            case HorizontalDirection.East:
                pos += Vector2Int.left * (oppositeExit.size.x / 2 - 1);
                break;
            case HorizontalDirection.South:
                pos += Vector2Int.up * (oppositeExit.size.y / 2 - 1);
                break;
        }

        GameManager.Instance.PrepareForPlayerSpawn(pos);
        //SpawnPlayer();

        //StartCoroutine(DelayedLoadFinish());
    }

    IEnumerator DelayedLoadFinish()
    {
        yield return null;
        loadingLevel = false;
    }
}
