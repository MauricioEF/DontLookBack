using UnityEngine;

public sealed class HorizontalTraversalProbe : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private CharacterMotor motor;
    [SerializeField] private GroundSensor groundSensor;
    [SerializeField] private Collider2D bodyCollider;

    [Header("Optional Camera Reference")]
    [SerializeField] private Camera scrollingCamera;

    [Header("Measurement")]
    [SerializeField, Min(0.1f)]
    private float maxRecordingDuration = 8f;

    private bool recording;
    private bool leftGround;

    private float startTime;
    private float startX;
    private float startFootY;
    private float maximumFootY;
    private float startCameraX;

    private void OnEnable()
    {
        if (inputReader == null ||
            motor == null ||
            groundSensor == null ||
            bodyCollider == null)
        {
            Debug.LogError(
                "Traversal probe is missing required references.",
                this
            );

            enabled = false;
            return;
        }

        inputReader.JumpPressed += OnJumpPressed;
    }

    private void OnDisable()
    {
        if (inputReader != null)
            inputReader.JumpPressed -= OnJumpPressed;

        recording = false;
    }

    private void OnJumpPressed()
    {
        // Do not restart an ongoing measurement when
        // the player performs an air jump.
        if (recording)
            return;

        if (motor.Mode != CharacterMotorMode.Platforming)
            return;

        if (!groundSensor.IsGrounded)
            return;

        recording = true;
        leftGround = false;

        startTime = Time.time;
        startX = bodyCollider.bounds.center.x;
        startFootY = bodyCollider.bounds.min.y;
        maximumFootY = startFootY;

        startCameraX = scrollingCamera != null
            ? scrollingCamera.transform.position.x
            : 0f;
    }

    private void FixedUpdate()
    {
        if (!recording)
            return;

        float currentFootY = bodyCollider.bounds.min.y;

        maximumFootY = Mathf.Max(
            maximumFootY,
            currentFootY
        );

        if (!groundSensor.IsGrounded)
            leftGround = true;

        if (leftGround &&
            groundSensor.IsGrounded &&
            motor.VerticalVelocity <= 0f)
        {
            CompleteMeasurement();
            return;
        }

        if (Time.time - startTime >= maxRecordingDuration)
        {
            Debug.LogWarning(
                "Traversal measurement ended without a landing.",
                this
            );

            recording = false;
        }
    }

    private void CompleteMeasurement()
    {
        float elapsed = Time.time - startTime;

        float landingX = bodyCollider.bounds.center.x;
        float landingFootY = bodyCollider.bounds.min.y;

        float horizontalDistance = landingX - startX;
        float maximumHeight = maximumFootY - startFootY;
        float landingHeightDifference = landingFootY - startFootY;

        float cameraDisplacement = scrollingCamera != null
            ? scrollingCamera.transform.position.x - startCameraX
            : 0f;

        float relativeDistance =
            horizontalDistance - cameraDisplacement;

        Debug.Log(
            $"TRAVERSAL MEASUREMENT\n" +
            $"Airtime: {elapsed:F3} s\n" +
            $"Horizontal distance: {horizontalDistance:F3} units\n" +
            $"Maximum height: {maximumHeight:F3} units\n" +
            $"Landing height difference: {landingHeightDifference:F3} units\n" +
            $"Camera-relative distance: {relativeDistance:F3} units",
            this
        );

        recording = false;
    }
}