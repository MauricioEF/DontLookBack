using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 3.0f;

    [Header("Mode")]
    [SerializeField]
    private CharacterMotorMode mode = CharacterMotorMode.Platforming;

    private Vector2 moveDirection;

    public CharacterMotorMode Mode => mode;
    public Vector2 Velocity => rb.linearVelocity;
    public float VerticalVelocity => rb.linearVelocity.y;

    public float GravityScale => rb.gravityScale;

    private void FixedUpdate()
    {
        switch (mode)
        {
            case CharacterMotorMode.Platforming:
                ApplyPlatformingMovement();
                break;
            case CharacterMotorMode.Directed2D:
                ApplyDirectedMovement();
                break;
        }
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    public void SetGravityScale(float gravityScale)
    {
        rb.gravityScale = Mathf.Max(0f, gravityScale);
    }

    public void ClearMoveDirection()
    {
        moveDirection = Vector2.zero;
    }

    public void SetMode(CharacterMotorMode newMode)
    {
        if (mode == newMode)
            return;
        mode = newMode;
        ClearMoveDirection();
    }

    public void SetVerticalVelocity(float velocity)
    {
        if (mode != CharacterMotorMode.Platforming)
            return;
        Vector2 currentVelocity = rb.linearVelocity;
        currentVelocity.y = velocity;
        rb.linearVelocity = currentVelocity;
    }

    private void ApplyPlatformingMovement()
    {
        Vector2 currentVelocity = rb.linearVelocity;

        float horizontalInput = Mathf.Clamp(moveDirection.x, -1f, 1f);
        currentVelocity.x = horizontalInput * moveSpeed;
        rb.linearVelocity = currentVelocity;
    }

    private void ApplyDirectedMovement()
    {
        Vector2 normalizedDirection = Vector2.ClampMagnitude(moveDirection, 1f);
        rb.linearVelocity = normalizedDirection * moveSpeed;
    }
    public void Stop()
    {
        moveDirection = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }
}
