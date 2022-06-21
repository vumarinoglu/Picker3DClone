using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Level level;
    public List<GameObject> roads;
    public List<GameObject> endings;

    public bool active;

    private void OnEnable()
    {
        SpawnManager.OnLevelSpawned += IsActive;
        GameManager.OnEndLevel += IsActive;
        GameManager.OnChapterWon += EndChapter;
    }

    private void OnDisable()
    {
        SpawnManager.OnLevelSpawned -= IsActive;
        GameManager.OnEndLevel -= IsActive;
        GameManager.OnChapterWon -= EndChapter;

        CloseBridges();
    }

    public void EndChapter(int chapter)
    {
        if (!active) return;

        var endingAnim = endings[chapter].GetComponent<Animator>();
        endingAnim.SetTrigger("raise");
    }

    public void IsActive()
    {
        if (GameManager.Instance.currentLevel == level.id)
        {
            active = true;
            PlayerPrefs.SetInt("lastPlayedLevelID", SpawnManager.Instance.realID);
        }
        else
        {
            active = false;
        }
    }

    public void CloseBridges()
    {
        for (int i = 0; i < 3; i++)
        {
            var endingAnim = endings[i].GetComponent<Animator>();
            endingAnim.SetTrigger("idle");
        }
    }
}
