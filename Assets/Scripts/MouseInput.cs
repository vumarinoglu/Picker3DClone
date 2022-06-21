using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    private float lastMousePos;
    private float _moveXFactor;
    public float moveXFactor => _moveXFactor;

    private void Update()
    {
        TakeInput();
    }

    public void TakeInput()
    {
        switch (GameManager.Instance.GetGameState())
        {
            case GameManager.GameState.START:

                if (Input.GetMouseButtonDown(0))
                {
                    GameManager.Instance.ChangeGameState(GameManager.GameState.GAMEPLAY);
                }

                break;
            case GameManager.GameState.GAMEPLAY:

                if (Input.GetMouseButtonDown(0))
                {
                    lastMousePos = Input.mousePosition.x;
                }
                else if (Input.GetMouseButton(0))
                {
                    _moveXFactor = Input.mousePosition.x - lastMousePos;
                    lastMousePos = Input.mousePosition.x;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    _moveXFactor = 0;
                }

                break;
            case GameManager.GameState.GAMEOVER:


                break;
            default:
                break;
        }

    }
}
