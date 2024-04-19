using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameInput;

[CreateAssetMenu(menuName = "InputReader_Player")]
public class InputReader_Player : ScriptableObject, IPlayerActions
{
    public event Action<Vector2> MoveEvent;
    public event Action<bool> FireEvent;

    private GameInput _playerInput;

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
        if (_playerInput == null)
        {
            _playerInput = new GameInput();
            _playerInput.Player.SetCallbacks(this);
        }
    }

    private void EnableInput()
    {
        _playerInput.Player.Enable();
    }

    private void DisableInput()
    {
        _playerInput.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        FireEvent?.Invoke(context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Performed);
    }

}
