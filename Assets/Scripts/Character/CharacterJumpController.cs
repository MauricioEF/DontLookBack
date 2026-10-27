using UnityEngine;

public sealed class CharacterJumpController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private CharacterMotor motor;
    [SerializeField] private GroundSensor groundSensor;

    [Header("Jump")]
    [SerializeField, Min(0f)] private float jumpVelocity = 7f;
    [SerializeField, Min(0f)] private float maxJumpHoldTime = 0.18f;
    [SerializeField, Min(0)] private int maxAirJumps = 1;

    [Header("Coyote Time")]
    [SerializeField, Min(0f)] private float coyoteTime = 0.1f;

    [Header("Gravity")]
    [SerializeField, Min(0f)] private float heldRiseGravityMultiplier = 1f;
    [SerializeField, Min(0f)] private float releasedRiseGravityMultiplier = 2.5f;
    [SerializeField, Min(0f)] private float fallGravityMultiplier = 2f;

    [SerializeField, Min(0f)] private float maxFallSpeed = 15f;


    private float baseGravityScale;
    private float coyoteRemaining;
    private float jumpHoldRemaining;
    private int remainingAirJumps;
    private bool isPlayerJumpAscent;
    private bool jumpSustainAvailable;

    public int MaxAirJumps => maxAirJumps;
    public int RemainingAirJumps => remainingAirJumps;

    private void Awake()
    {
        baseGravityScale = motor.GravityScale;
        remainingAirJumps = maxAirJumps;
    }

    private void OnEnable()
    {
        inputReader.JumpPressed += OnJumpPressed;
        inputReader.JumpReleased += OnJumpReleased;
    }

    private void OnDisable()
    {
        inputReader.JumpPressed -= OnJumpPressed;
        inputReader.JumpReleased -= OnJumpReleased;

        ResetJumpAscent();

        if (motor != null && motor.Mode == CharacterMotorMode.Platforming)
        {
            motor.SetGravityScale(baseGravityScale);
        }
    }

    private void Update()
    {
        if (motor.Mode != CharacterMotorMode.Platforming)
            return;
        bool isGrounded = IsGroundedForJumpRules();

        UpdateCoyoteAndAirJumps(isGrounded);
        UpdateGravity(isGrounded);
    }

    private void FixedUpdate()
    {
        if (motor.Mode != CharacterMotorMode.Platforming)
            return;
        if (motor.VerticalVelocity < -maxFallSpeed)
            motor.SetVerticalVelocity(-maxFallSpeed);
    }

    private void OnJumpPressed()
    {
        if (motor.Mode != CharacterMotorMode.Platforming)
            return;
        bool isGrounded = IsGroundedForJumpRules();

        //Covering the case where we land after the previous Update but pressing Jump immediately.
        if (isGrounded)
        {
            remainingAirJumps = maxAirJumps;
            coyoteRemaining = coyoteTime;
        }

        bool canGroundJump = isGrounded || coyoteRemaining > 0f;

        if (canGroundJump)
        {
            //Different presses shouldn't reuse the same coyote window.
            coyoteRemaining = 0f;
            PerformJump();
            return;
        }

        if (remainingAirJumps <= 0)
            return;
        remainingAirJumps--;
        PerformJump();
    }

    private void OnJumpReleased()
    {
        //Once released, holding jump again must not resume the original jump's sustain.
        jumpSustainAvailable = false;
    }

    private void PerformJump()
    {
        isPlayerJumpAscent = true;
        jumpSustainAvailable = inputReader.JumpHeld;
        jumpHoldRemaining = maxJumpHoldTime;

        motor.SetVerticalVelocity(jumpVelocity);
        motor.SetGravityScale(baseGravityScale * heldRiseGravityMultiplier);
    }

    private void UpdateCoyoteAndAirJumps(bool isGrounded)
    {
        if (isGrounded)
        {
            coyoteRemaining -= coyoteTime;
            remainingAirJumps = maxAirJumps;
            ResetJumpAscent();
            return;
        }
        coyoteRemaining = Mathf.Max(0f, coyoteRemaining - Time.deltaTime);
    }

    private void UpdateGravity(bool isGrounded)
    {
        if (isGrounded)
        {
            motor.SetGravityScale(baseGravityScale);
            return;
        }
        float verticalVelocity = motor.VerticalVelocity;

        if (verticalVelocity > 0f)
        {
            UpdateRisingGravity();
            return;
        }

        //We reached the limit or we're falling starting from here.
        ResetJumpAscent();

        motor.SetGravityScale(baseGravityScale * fallGravityMultiplier);
    }


    private void UpdateRisingGravity()
    {
        if (!isPlayerJumpAscent)
        {
            motor.SetGravityScale(baseGravityScale * heldRiseGravityMultiplier);
            return;
        }
        bool canSustainJump = jumpSustainAvailable && inputReader.JumpHeld && jumpHoldRemaining > 0f;

        if (canSustainJump)
        {
            jumpHoldRemaining = Mathf.Max(0f, jumpHoldRemaining - Time.deltaTime);
            if (jumpHoldRemaining <= 0f)
            {
                jumpSustainAvailable = false;
            }
            motor.SetGravityScale(baseGravityScale * heldRiseGravityMultiplier);
            return;
        }
        jumpSustainAvailable = false;
        motor.SetGravityScale(baseGravityScale * releasedRiseGravityMultiplier);
    }

    private bool IsGroundedForJumpRules()
    {
        if (!groundSensor.IsGrounded)
            return false;

        //Validation: the GroundSensor can still touch the ground for an instant after jumping.
        //The validation prevents the jump reset while we're actually ascending from a player jump.
        if (isPlayerJumpAscent && motor.VerticalVelocity > 0f)
        {
            return false;
        }
        return true;
    }

    private void ResetJumpAscent()
    {
        isPlayerJumpAscent = false;
        jumpSustainAvailable = false;
        jumpHoldRemaining = 0f;
    }

    private void SetMaxAirJumps(int value)
    {
        int newMaximum = Mathf.Max(0, value);
        int userAirJumps = Mathf.Max(0, maxAirJumps - remainingAirJumps);
        maxAirJumps = newMaximum;
        remainingAirJumps = Mathf.Max(0, maxAirJumps - userAirJumps);

        if (IsGroundedForJumpRules())
        {
            remainingAirJumps = maxAirJumps;
        }
    }
}
