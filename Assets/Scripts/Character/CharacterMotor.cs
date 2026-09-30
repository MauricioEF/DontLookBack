using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 3.0f;

    private Vector2 moveDirection;
    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = Vector2.ClampMagnitude(direction, 1f);
    }

    public void Stop()
    {
        moveDirection = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }
}
