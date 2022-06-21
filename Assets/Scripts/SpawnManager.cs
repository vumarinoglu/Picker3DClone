using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    private Level level;

    [SerializeField]
    private int builtLevelAmount;

    [SerializeField]
    private int levelID;

    public int realID;

    [SerializeField]
    private GameObject levelPrefab;

    [SerializeField]
    private float levelLength = 95f;

    [SerializeField]
    private List<GameObject> collectiblePrefabs;

    [SerializeField]
    private List<GameObject> activeLevels = new List<GameObject>();

    private Transform currentLevel;
    private GamePools gamePools;

    public static Action OnLevelSpawned;
    public static Action<int> OnChapterSpawned;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    private void OnEnable()
    {
        GameManager.OnEndLevel += NextLevel;
    }

    private void OnDisable()
    {
        GameManager.OnEndLevel -= NextLevel;
    }

    // Start is called before the first frame update
    void Start()
    {
        gamePools = GameManager.Instance.gamePools;

        levelID = PlayerPrefs.GetInt("currentLevel", 0);

        if(levelID >= builtLevelAmount)
        {
            levelID = PlayerPrefs.GetInt("lastPlayedLevelID", UnityEngine.Random.Range(0, builtLevelAmount));
        }

        CreateLevel(levelID);
        CreateLevel(levelID+1);
    }

    public Level GetLevel() => level;

    private void CreateLevel(int levelID)
    {
        GameObject levelObj;

        if(levelID >= builtLevelAmount)
        {
            levelID = UnityEngine.Random.Range(0, builtLevelAmount);
        }

        var serializer = new fsSerializer();
        level = FileUtils.LoadJsonFile<Level>(serializer, "Levels/" + levelID);

        if (currentLevel == null)
        {
            var startPos = new Vector3(0.0f, 0.0f, 10f);
            levelObj = Instantiate(levelPrefab, startPos, levelPrefab.transform.rotation);
            realID = level.id;
            level.id = GameManager.Instance.currentLevel;
        }
        else
        {
            levelObj = Instantiate(levelPrefab, currentLevel.position + new Vector3(0.0f, 0.0f, levelLength), levelPrefab.transform.rotation);
            realID = level.id-1;
            level.id = GameManager.Instance.currentLevel + 1;
        }

        currentLevel = levelObj.transform;
        activeLevels.Add(levelObj);

        var levelManager = levelObj.GetComponent<LevelManager>();
        levelManager.level = level;

        var roads = levelManager.roads;

        for (int h = 0; h < 3; h++)
        {
            Vector3 spawnStartPoint = new Vector3(-4.5f, 1.0f, 9.5f);
            Vector3 currentSpawnPoint = spawnStartPoint;
            for (var j = 0; j < level.length; j++)
            {
                for (var i = 0; i < level.width; i++)
                {
                    var tileIndex = i + (j * level.width);
                    if(h == 0)
                    {
                        var tile = level.firstChapterTiles[tileIndex];

                        var obj = GetTileEntity(tile, roads[h].transform, currentSpawnPoint, new Quaternion(0f,0f,0f,0f));
                    }
                    else if (h == 1)
                    {
                        var tile = level.secondChapterTiles[tileIndex];

                        var obj = GetTileEntity(tile, roads[h].transform, currentSpawnPoint, new Quaternion(0f, 0f, 0f, 0f));
                    }
                    else if (h == 2)
                    {
                        var tile = level.finalChapterTiles[tileIndex];

                        var obj = GetTileEntity(tile, roads[h].transform, currentSpawnPoint, new Quaternion(0f, 0f, 0f, 0f));
                    }

                    currentSpawnPoint.x += 1.0f;
                }
                currentSpawnPoint.x = -4.5f;
                currentSpawnPoint.z -= 1.0f;
            }
            OnChapterSpawned?.Invoke(h);
        }

        OnLevelSpawned?.Invoke();
    }

    public void NextLevel()
    {
        if(level.id + 1 > builtLevelAmount)
        {
            var rand = UnityEngine.Random.Range(0, builtLevelAmount);
            CreateLevel(rand);
        }
        else
        {
            CreateLevel(level.id + 1);
        }

        if(level.id - 2 >= 0 && activeLevels.Count > 3)
        {
            Destroy(activeLevels[0]);
        }
    }


    private GameObject GetTileEntity(LevelTile tile, Transform parent, Vector3 position, Quaternion rotation)
    {
        if(tile != null)
        {
            if(tile is CollectibleTile)
            {
                var item = (CollectibleTile)tile;

                switch (item.type)
                {
                    case CollectibleType.S_SPHERE:
                        return gamePools.smallSpherePool.GetObject(parent, position, rotation);
                    case CollectibleType.S_CUBE:
                        return gamePools.smallCubePool.GetObject(parent, position, rotation);
                    case CollectibleType.S_CAPSULE:
                        return gamePools.smallCapsulePool.GetObject(parent, position, rotation);
                    case CollectibleType.M_SPHERE:
                        return gamePools.mediumSpherePool.GetObject(parent, position, rotation);
                    case CollectibleType.M_CUBE:
                        return gamePools.mediumCubePool.GetObject(parent, position, rotation);
                    case CollectibleType.M_CAPSULE:
                        return gamePools.mediumCapsulePool.GetObject(parent, position, rotation);
                    case CollectibleType.L_SPHERE:
                        return gamePools.largeSpherePool.GetObject(parent, position, rotation);
                    case CollectibleType.L_CUBE:
                        return gamePools.largeCubePool.GetObject(parent, position, rotation);
                    case CollectibleType.L_CAPSULE:
                        return gamePools.largeCapsulePool.GetObject(parent, position, rotation);
                    default:
                        return gamePools.smallSpherePool.GetObject(parent, position, rotation);
                }
            }
            else if (tile is BoosterTile)
            {
                var item = (BoosterTile)tile;

                switch (item.type)
                {
                    case BoosterType.WINGS:
                        return gamePools.wingsPool.GetObject(parent, position, rotation);
                    case BoosterType.SIZEUP:
                        break;
                    default:
                        break;
                }
            }
        }

        return null;
    }
}
