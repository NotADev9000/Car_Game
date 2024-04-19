using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameInput;

[CreateAssetMenu(menuName = "InputReader_UI")]
public class InputReader_UI : ScriptableObject, IUIActions
{
    public event Action<bool> PauseEvent;

    private GameInput _uiInput;

    private void OnEnable()
    {
        GameManager.OnGameStarted += EnableInput;
        GameManager.OnGameEnded += DisableInput;
        SetupInput();
    }

    private void OnDisable()
    {
        GameManager.OnGameStarted -= EnableInput;
        GameManager.OnGameEnded -= DisableInput;
    }

    private void SetupInput()
    {
        if (_uiInput == null)
        {
            _uiInput = new GameInput();
            _uiInput.UI.SetCallbacks(this);
        }
    }

    private void EnableInput()
    {
        _uiInput.UI.Enable();
    }

    private void DisableInput()
    {
        _uiInput.UI.Disable();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        PauseEvent?.Invoke(context.phase == InputActionPhase.Started);
    }
}
