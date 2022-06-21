using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GamePools gamePools;

    public static Action OnStartLevel;
    public static Action OnEndOfChapter;
    public static Action<int> OnChapterWon;
    public static Action OnChapterLost;
    public static Action OnEndLevel;
    public static Action OnNextLevel;

    public static Action OnGamePlayStarted;
    public static Action OnGameOver;

    public int currentLevel;

    private GameState currentState;
    public enum GameState
    {
        START,
        GAMEPLAY,
        GAMEOVER
    }

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;

        currentLevel = PlayerPrefs.GetInt("currentLevel", 0);
    }

    private void OnEnable()
    {
        OnEndLevel += IncreaseLevel;
        OnChapterLost += LevelLost;
    }

    private void OnDisable()
    {
        OnEndLevel -= IncreaseLevel;
        OnChapterLost -= LevelLost;
    }

    void Start()
    {
        ChangeGameState(GameState.START);
    }

    public GameState GetGameState() => currentState;
    public void ChangeGameState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.START:
                break;
            case GameState.GAMEPLAY:
                OnGamePlayStarted?.Invoke();
                break;
            case GameState.GAMEOVER:
                OnGameOver?.Invoke();
                break;
            default:
                break;
        }
    }

    public void IncreaseLevel()
    {
        currentLevel++;
        PlayerPrefs.SetInt("currentLevel", currentLevel);
    }

    public void LevelLost()
    {
        ChangeGameState(GameState.GAMEOVER);
    }
}
