using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField]
    private Level level;

    [SerializeField]
    private int levelID;

    [SerializeField]
    private GameObject levelPrefab;

    [SerializeField]
    private float levelLength = 95f;

    private Transform prevLevel;

    [SerializeField]
    private List<GameObject> collectiblePrefabs;

    private Dictionary<CollectibleType, GameObject> dict = new Dictionary<CollectibleType, GameObject>();

    public static Action OnLevelSpawned;
    public static Action<int> OnChapterSpawned;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        dict.Add(CollectibleType.S_SPHERE, collectiblePrefabs[0]);
        dict.Add(CollectibleType.M_SPHERE, collectiblePrefabs[1]);
        dict.Add(CollectibleType.L_SPHERE, collectiblePrefabs[2]);
        dict.Add(CollectibleType.S_CUBE, collectiblePrefabs[3]);
        dict.Add(CollectibleType.M_CUBE, collectiblePrefabs[4]);
        dict.Add(CollectibleType.L_CUBE, collectiblePrefabs[5]);
        dict.Add(CollectibleType.S_CAPSULE, collectiblePrefabs[6]);
        dict.Add(CollectibleType.M_CAPSULE, collectiblePrefabs[7]);
        dict.Add(CollectibleType.L_CAPSULE, collectiblePrefabs[8]);

        CreateLevel(levelID);
    }

    public Level GetLevel() => level;

    private void CreateLevel(int levelID)
    {
        GameObject levelObj;

        var serializer = new fsSerializer();
        level = FileUtils.LoadJsonFile<Level>(serializer, "Levels/" + levelID);

        if (prevLevel == null)
        {
            levelObj = Instantiate(levelPrefab);
        }
        else
        {
            levelObj = Instantiate(levelPrefab, prevLevel.position + new Vector3(0.0f, 0.0f, levelLength), levelPrefab.transform.rotation);
        }

        prevLevel = levelObj.transform;

        var levelParts = levelObj.GetComponent<LevelParts>();

        var roads = levelParts.chapters;


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

                        var item = GetTileEntity(tile);

                        if(item != null)
                        {
                            var obj = Instantiate(item);
                            obj.transform.parent = roads[h].transform;
                            obj.transform.localPosition = currentSpawnPoint;
                        }
                    }
                    else if (h == 1)
                    {
                        var tile = level.secondChapterTiles[tileIndex];

                        var item = GetTileEntity(tile);

                        if (item != null)
                        {
                            var obj = Instantiate(item);
                            obj.transform.parent = roads[h].transform;
                            obj.transform.localPosition = currentSpawnPoint;
                        }
                    }
                    else if (h == 2)
                    {
                        var tile = level.finalChapterTiles[tileIndex];

                        var item = GetTileEntity(tile);

                        if (item != null)
                        {
                            var obj = Instantiate(item);
                            obj.transform.parent = roads[h].transform;
                            obj.transform.localPosition = currentSpawnPoint;
                        }
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

    private GameObject GetTileEntity(LevelTile tile)
    {
        if(tile != null)
        {
            if(tile is CollectibleTile)
            {
                var item = (CollectibleTile)tile;

                switch (item.type)
                {
                    case CollectibleType.S_SPHERE:
                        return dict[CollectibleType.S_SPHERE];
                    case CollectibleType.S_CUBE:
                        return dict[CollectibleType.S_CUBE];
                    case CollectibleType.S_CAPSULE:
                        return dict[CollectibleType.S_CAPSULE];
                    case CollectibleType.M_SPHERE:
                        return dict[CollectibleType.M_SPHERE];
                    case CollectibleType.M_CUBE:
                        return dict[CollectibleType.M_CUBE];
                    case CollectibleType.M_CAPSULE:
                        return dict[CollectibleType.M_CAPSULE];
                    case CollectibleType.L_SPHERE:
                        return dict[CollectibleType.L_SPHERE];
                    case CollectibleType.L_CUBE:
                        return dict[CollectibleType.L_CUBE];
                    case CollectibleType.L_CAPSULE:
                        return dict[CollectibleType.L_CAPSULE];
                    default:
                        return dict[CollectibleType.S_SPHERE];
                }
            }
        }

        return null;
    }
}
