using UnityEngine;

public class Oscillator : MonoBehaviour
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    public enum OscillationSpeed
    {
        Slow,
        Normal,
        Fast
    }

    [Header("Rotation Settings")]
    [SerializeField] private RotationAxis rotationAxis = RotationAxis.Z;

    [Tooltip("Angle offset relative to the initial rotation.")]
    [SerializeField] private float firstAngle = 45f;

    [Tooltip("Enable a second angle instead of using the initial rotation.")]
    [SerializeField] private bool useSecondAngle = false;

    [Tooltip("Second angle offset relative to the initial rotation.")]
    [SerializeField] private float secondAngle = -45f;

    [Header("Oscillation Settings")]
    [SerializeField] private OscillationSpeed speed = OscillationSpeed.Normal;

    private Quaternion initialRotation;
    private float elapsedTime;

    private void Awake()
    {
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        float speedMultiplier = speed switch
        {
            OscillationSpeed.Slow => 0.5f,
            OscillationSpeed.Normal => 1f,
            OscillationSpeed.Fast => 2f,
            _ => 1f
        };

        float minAngle = useSecondAngle ? secondAngle : 0f;
        float maxAngle = firstAngle;

        // Oscillate smoothly between 0 and 1.
        float t = (Mathf.Sin(elapsedTime * speedMultiplier) + 1f) / 2f;

        float currentAngle = Mathf.Lerp(minAngle, maxAngle, t);

        Vector3 axis = rotationAxis switch
        {
            RotationAxis.X => Vector3.right,
            RotationAxis.Y => Vector3.up,
            RotationAxis.Z => Vector3.forward,
            _ => Vector3.forward
        };

        transform.localRotation =
            initialRotation * Quaternion.AngleAxis(currentAngle, axis);
    }
}
