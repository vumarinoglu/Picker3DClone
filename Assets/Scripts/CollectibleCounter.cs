using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectibleCounter : MonoBehaviour
{
    [SerializeField]
    private int chapter;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private int score;

    [SerializeField]
    private int goal;

    private bool countdown = false;

    [SerializeField]
    private float countdownTime = 3f;
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
            GameManager.OnChapterLost?.Invoke();
            countdown = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Collectible"))
        {
            countdown = true;
            Destroy(collision.gameObject);
            AddScore();
        }
    }

    public void AddScore()
    {
        score++;
        scoreText.text = score + "/" + goal;
        currentTime = countdownTime;

        if(score >= goal)
        {
            countdown = false;
            GameManager.OnChapterWon?.Invoke();
        }
    }

    public void GetChapterScore()
    {
        switch (chapter)
        {
            case 0:
                goal = SpawnManager.Instance.GetLevel().firstGoal;
                scoreText.text = "0/" + goal;
                break;
            case 1:
                goal = SpawnManager.Instance.GetLevel().secondGoal;
                scoreText.text = "0/" + goal;
                break;
            case 2:
                goal = SpawnManager.Instance.GetLevel().finalGoal;
                scoreText.text = "0/" + goal;
                break;
        }
    }
}
