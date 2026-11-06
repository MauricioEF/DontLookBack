using System;
using System.Collections.Generic;
using UnityEngine;

public enum PatternTraversalRequirement
{
    Walk = 0,
    GroundJump = 1,
    AirJump = 2
}

[Serializable]
public sealed class FloorSpan
{
    [SerializeField, Min(0f)] private float startX;
    [SerializeField, Min(0f)] private float endX;
    [SerializeField] private float surfaceY;

    public float StartX => startX;
    public float EndX => endX;
    public float SurfaceY => surfaceY;
}

[CreateAssetMenu(fileName = "GenerationPattern", menuName = "Scriptable Objects/GenerationPattern")]
public class GenerationPattern : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string patternId;

    [Header("Objects")]
    [SerializeField] private List<PatternObjectPlacement> objects = new();

    [Header("Horizontal Floor Topology")]
    [SerializeField] private List<FloorSpan> floorSpans = new();

    [Header("Traversal")]
    [SerializeField] private PatternTraversalRequirement requiredTraversal = PatternTraversalRequirement.Walk;

    private float boundaryTolerance = 0.001f;

    public string PatternId => patternId;
    public IReadOnlyList<PatternObjectPlacement> Objects => objects;

    public IReadOnlyList<FloorSpan> FloorSpans => floorSpans;

    public PatternTraversalRequirement RequiredTraversal => requiredTraversal;


    public bool TryGetGroundedEntry(out float surfaceY)
    {
        surfaceY = 0f;

        foreach (FloorSpan span in floorSpans)
        {
            if (span == null || span.EndX <= span.StartX)
                continue;

            //Entry is the left boundary of the pattern.
            if (Mathf.Abs(span.StartX) > boundaryTolerance)
                continue;

            surfaceY = span.SurfaceY;
            return true;
        }
        return false;
    }

    public bool TryGetGroundedExit(float regionWidth, out float surfaceY)
    {
        surfaceY = 0f;
        if (regionWidth <= 0f)
            return false;

        foreach (FloorSpan span in floorSpans)
        {
            if (span == null || span.EndX <= span.StartX)
                continue;

            //Exit is the right boundary of the pattern.
            if (Math.Abs(span.EndX - regionWidth) > boundaryTolerance)
                continue;
            surfaceY = span.SurfaceY;
            return true;
        }
        return false;
    }
}

[Serializable]
public sealed class PatternObjectPlacement
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector2 localPosition;

    public GameObject Prefab => prefab;
    public Vector2 LocalPosition => localPosition;
}
