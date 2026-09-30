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
    public void OnEnable()
    {
        actions = CoreRuntime.Instance.Input.Actions;

        actions.Gameplay.Movement.performed += OnMovePerformed;
        actions.Gameplay.Movement.canceled += OnMoveCanceled;
    }
    public void OnDisable()
    {
        if (actions == null)
            return;
        actions.Gameplay.Movement.performed -= OnMovePerformed;
        actions.Gameplay.Movement.canceled -= OnMoveCanceled;

        Move = Vector2.zero;
        actions = null;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Move = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        Move = Vector2.zero;
    }
}
