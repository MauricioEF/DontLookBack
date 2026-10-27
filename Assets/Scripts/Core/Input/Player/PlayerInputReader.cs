using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    private InputActions actions;
    public Vector2 Move
    {
        get;
        private set;
    }

    public bool JumpHeld
    {
        get;
        private set;
    }

    public event Action JumpPressed;
    public event Action JumpReleased;

    public void OnEnable()
    {
        actions = CoreRuntime.Instance.Input.Actions;

        actions.Gameplay.Movement.performed += OnMovePerformed;
        actions.Gameplay.Movement.canceled += OnMoveCanceled;

        actions.Gameplay.Jump.performed += OnJumpPerformed;
        actions.Gameplay.Jump.canceled += OnJumpCanceled;
    }
    public void OnDisable()
    {
        if (actions == null)
            return;
        actions.Gameplay.Movement.performed -= OnMovePerformed;
        actions.Gameplay.Movement.canceled -= OnMoveCanceled;

        actions.Gameplay.Jump.performed -= OnJumpPerformed;
        actions.Gameplay.Jump.canceled -= OnJumpCanceled;

        Move = Vector2.zero;
        JumpHeld = false;
        actions = null;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        Move = Vector2.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (JumpHeld)
            return;
        JumpHeld = true;
        JumpPressed?.Invoke();
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if (!JumpHeld)
            return;
        JumpHeld = false;
        JumpReleased?.Invoke();
    }
}
