using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectibleCounter : MonoBehaviour
{
    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private int chapter;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private int score;

    [SerializeField]
    private int goal;

    [SerializeField]
    private float countdownTime = 1.5f;

    private bool countdown = false;
    private float currentTime;

    private void OnEnable()
    {
        SpawnManager.OnLevelSpawned += GetChapterScore;
    }

    private void OnDisable()
    {
        SpawnManager.OnLevelSpawned -= GetChapterScore;
    }

    private void Start()
    {
        countdown = false;
        currentTime = countdownTime;
    }

    private void Update()
    {
        if (!countdown) return;

        currentTime -= Time.deltaTime;
        if(currentTime <= 0)
        {
            if(score < goal)
            {
                GameManager.OnChapterLost?.Invoke();
                countdown = false;
            }
            else
            {
                GameManager.OnChapterWon?.Invoke(chapter);
                countdown = false;
                if(chapter == 2)
                {
                    StartCoroutine(EndLevelAsync());
                }
            }
        }
    }

    IEnumerator EndLevelAsync()
    {
        yield return new WaitForSeconds(.25f);
        GameManager.OnEndLevel?.Invoke();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Collectible"))
        {
            countdown = true;
            AddScore();
        }
    }

    public void AddScore()
    {
        score++;
        scoreText.text = score + "/" + goal;
        currentTime = countdownTime;
    }

    public void GetChapterScore()
    {
        switch (chapter)
        {
            case 0:
                goal = levelManager.level.firstGoal;
                scoreText.text = "0/" + goal;
                break;
            case 1:
                goal = levelManager.level.secondGoal;
                scoreText.text = "0/" + goal;
                break;
            case 2:
                goal = levelManager.level.finalGoal;
                scoreText.text = "0/" + goal;
                break;
        }
    }
}
