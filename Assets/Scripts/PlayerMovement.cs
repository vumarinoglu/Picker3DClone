using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    [SerializeField]
    private MouseInput mouseInput;

    [SerializeField]
    private float minX, maxX;

    private void OnEnable()
    {
        GameManager.OnEndOfChapter += StopMovement;
        GameManager.OnChapterWon += StartMovement;
        GameManager.OnChapterLost += StopMovement;
        GameManager.OnGamePlayStarted += StartMovement;
        GameManager.OnEndLevel += StopMovement;
        GameManager.OnNextLevel += StartMovement;
    }

    private void OnDisable()
    {
        GameManager.OnEndOfChapter -= StopMovement;
        GameManager.OnChapterWon -= StartMovement;
        GameManager.OnChapterLost -= StopMovement;
        GameManager.OnGamePlayStarted -= StartMovement;
        GameManager.OnEndLevel -= StopMovement;
        GameManager.OnNextLevel += StartMovement;
    }

    private void Update()
    {
        switch (GameManager.Instance.GetGameState())
        {
            case GameManager.GameState.START:

                isActive = false;

                break;
            case GameManager.GameState.GAMEPLAY:

                transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX),
                    transform.position.y, transform.position.z);

                break;
            case GameManager.GameState.GAMEOVER:

                isActive = false;

                break;
            default:
                break;
        }


    }

    private void FixedUpdate()
    {
        if(!isActive) return;

        Move(new Vector3(mouseInput.moveXFactor, 0f, 1f));
    }

    public void StopMovement()
    {
        isActive = false;
    }

    public void StartMovement(int chapter)
    {
        isActive = true;
    }

    public void StartMovement()
    {
        isActive = true;
    }
}
