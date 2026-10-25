using System.Collections.Generic;
using UnityEngine;

public class HorizontalGameplayRegionLifecycle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform generatedRegionsRoot;


    [Header("Region Settings")]
    [SerializeField, Min(0.01f)] private float regionWidth = 16f;
    [SerializeField] private float gridOriginX = 0f;
    [SerializeField] private Transform generationEntry;

    [Header("Coverage")]
    [SerializeField, Min(0f)] private float aheadBuffer = 16f;
    [SerializeField, Min(0f)] private float behindBuffer = 16f;

    [Header("Pattern Selection")]
    [SerializeField] private GenerationPatternSelector patternSelector;

    private readonly Queue<GameplayRegion> activeRegions = new();
    private readonly Stack<GameplayRegion> availableRegions = new();

    private float nextRegionStartX;
    private int nextRegionId;
    private bool initialized;

    public int ActiveRegionCount => activeRegions.Count;
    public int AvailableRegionCount => availableRegions.Count;
    public float NextRegionStartX => nextRegionStartX;

    public void Initialize()
    {
        if (initialized)
            return;
        if (targetCamera == null || generatedRegionsRoot == null)
        {
            Debug.LogError("Gameplay regions require a Camera and GeneratedRegions root.", this);
            return;
        }

        if (regionWidth <= 0f)
        {
            Debug.LogError("Region width must be positive. ", this);
            return;
        }

        if (patternSelector == null || !patternSelector.HasValidPatterns)
        {
            Debug.LogError("PatternSelector not found or with non valid patterns", this);
            return;
        }

        nextRegionStartX = gridOriginX + Mathf.Ceil((generationEntry.position.x - gridOriginX) / regionWidth) * regionWidth;
        patternSelector.ResetSequence();
        initialized = true;
        EnsureCoverage();
    }
    private void LateUpdate()
    {
        if (!initialized)
            return;
        RetireExpiredRegions();
        EnsureCoverage();
    }


    private void RetireExpiredRegions()
    {
        float cleanupBoundary = GetCameraLeftEdge() - behindBuffer;

        while (activeRegions.Count > 0)
        {
            GameplayRegion oldest = activeRegions.Peek();

            float regionEndX = oldest.transform.position.x + regionWidth;

            if (regionEndX > cleanupBoundary)
                break;
            activeRegions.Dequeue();

            //The region contains no gameplay objects yet.
            oldest.gameObject.SetActive(false);
            oldest.Clear();
            availableRegions.Push(oldest);
        }
    }

    private void EnsureCoverage()
    {
        float requiredRightEdge = GetCameraRightEdge() + aheadBuffer;

        while (nextRegionStartX < requiredRightEdge)
        {

            if (!patternSelector.TrySelectNext(out GenerationPattern pattern))
            {
                Debug.LogError("Unable to select a generation pattern.", this);
                enabled = false;
                return;
            }

            GameplayRegion region = AcquireRegion();

            region.transform.position = new Vector3(nextRegionStartX, generatedRegionsRoot.position.y, generatedRegionsRoot.position.z);

            //Configuring content BEFORE making it visible

            region.Prepare(pattern);
            region.gameObject.SetActive(true);

            activeRegions.Enqueue(region);

            nextRegionStartX += regionWidth;
        }
    }

    private GameplayRegion AcquireRegion()
    {
        if (availableRegions.Count > 0)
            return availableRegions.Pop();

        GameObject regionObject = new GameObject($"GameplayRegion_{nextRegionId++}");

        regionObject.SetActive(false);

        regionObject.transform.SetParent(generatedRegionsRoot, false);

        return regionObject.AddComponent<GameplayRegion>();
    }

    private float GetCameraLeftEdge()
    {
        return targetCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, GetCameraPlaneDistance())).x;
    }

    private float GetCameraRightEdge()
    {
        return targetCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, GetCameraPlaneDistance())).x;
    }

    private float GetCameraPlaneDistance()
    {
        return Mathf.Abs(generatedRegionsRoot.position.z - targetCamera.transform.position.z);
    }

    //DEBUG ONLY
    private void OnDrawGizmos()
    {
        if (!initialized)
            return;
        Gizmos.color = Color.cyan;

        float regionHeight = 12f;
        foreach (GameplayRegion region in activeRegions)
        {
            Vector3 center = new Vector3(region.transform.position.x + regionWidth * 0.5f, region.transform.position.y, region.transform.position.z);
            Gizmos.DrawWireCube(center, new Vector3(regionWidth, regionHeight, 0f));
        }
    }
}