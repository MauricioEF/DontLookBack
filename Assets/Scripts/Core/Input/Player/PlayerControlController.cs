using UnityEngine;

public sealed class PlayerControlController : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private CharacterMotor controlledMotor;

    private void Update()
    {
        if (controlledMotor == null)
            return;
        controlledMotor.SetMoveDirection(inputReader.Move);
    }

    private void OnDisable()
    {
        controlledMotor?.Stop();
    }
}
