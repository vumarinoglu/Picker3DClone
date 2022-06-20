using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static Action OnStartLevel;
    public static Action OnEndOfChapter;
    public static Action OnChapterWon;
    public static Action OnChapterLost;

    private int currentChapter;

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
    }

    private void OnEnable()
    {
        OnChapterWon += NextChapter;
    }

    private void OnDisable()
    {
        OnChapterWon -= NextChapter;
    }

    void Start()
    {
        currentState = GameState.START;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameState GetGameState() => currentState;
    public void ChangeGameState(GameState newState)
    {
        currentState = newState;
    }

    public void NextChapter()
    {
        if(currentChapter < 2)
        {
            currentChapter++;
        }
        else
        {
            currentChapter = 0;
        }
    }
}
