using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private int chapter;

    private void OnEnable()
    {
        GameManager.OnChapterWon += ReturnCollectibles;
    }

    private void OnDisable()
    {
        GameManager.OnChapterWon -= ReturnCollectibles;
    }

    public void ReturnCollectibles(int chapter)
    {
        if (!levelManager.active) return;

        if (chapter != this.chapter) return;

        var objects = GetComponentsInChildren<PooledObject>();

        foreach (var obj in objects)
        {
            obj.pool.ReturnObject(obj.gameObject);
        }
    }
}
