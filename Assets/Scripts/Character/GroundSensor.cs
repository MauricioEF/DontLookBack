using UnityEngine;

public sealed class GroundSensor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D bodyCollider;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask terrainMask;
    [SerializeField, Min(0.001f)] private float probeDistance = 0.08f;
    [SerializeField, Range(0.1f, 1f)] private float probeWidthFactor = 0.85f;
    [SerializeField, Range(0f, 89f)] private float maximumGroundAngle = 50f;

    private readonly RaycastHit2D[] hits = new RaycastHit2D[8];

    public bool IsGrounded
    {
        get;
        private set;
    }

    public Vector2 GroundNormal
    {
        get;
        private set;
    } = Vector2.up;

    public Collider2D GroundCollider
    {
        get;
        private set;
    }

    public void FixedUpdate()
    {
        EvaluateGround();
    }

    private void EvaluateGround()
    {
        IsGrounded = false;
        GroundNormal = Vector2.up;
        GroundCollider = null;
        if (bodyCollider == null)
            return;
        Bounds bounds = bodyCollider.bounds;
        float probeWidth = bounds.size.x * probeWidthFactor;

        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y);

        Vector2 probeSize = new Vector2(probeWidth, 0.02f);

        ContactFilter2D filter = new ContactFilter2D();

        filter.SetLayerMask(terrainMask);
        filter.useTriggers = false;

        int hitCount = Physics2D.BoxCast(origin, probeSize, 0f, Vector2.down, filter, hits, probeDistance);

        float closestValidDistance = float.PositiveInfinity;

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.collider == null)
                continue;
            float groundAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (groundAngle > maximumGroundAngle)
                continue;
            if (hit.distance >= closestValidDistance)
                continue;
            closestValidDistance = hit.distance;
            IsGrounded = true;
            GroundNormal = hit.normal;
            GroundCollider = hit.collider;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (bodyCollider == null)
            return;
        Bounds bounds = bodyCollider.bounds;
        float probeWidth = bounds.size.x * probeWidthFactor;

        Vector3 center = new Vector3(bounds.center.x, bounds.min.y - probeDistance * 0.5f, transform.position.z);
        Vector3 size = new Vector3(probeWidth, probeDistance, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}
