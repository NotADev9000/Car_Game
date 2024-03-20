using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameInput;

[CreateAssetMenu(menuName = "InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public event Action<Vector2> MoveEvent;
    public event Action<bool> FireEvent;

    private GameInput _playerInput;

    private void OnEnable()
    {
        if (_playerInput == null)
        {
            _playerInput = new GameInput();
            _playerInput.Player.SetCallbacks(this);

            _playerInput.Player.Enable();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        FireEvent?.Invoke(context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Performed);
    }

}
