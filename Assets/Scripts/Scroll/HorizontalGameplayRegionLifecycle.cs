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

    private readonly Queue<Transform> activeRegions = new();
    private readonly Stack<Transform> availableRegions = new();

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

        float leftEdge = GetCameraLeftEdge();

        //Align the initial region to the configured world grid.

        float firstRegionX = gridOriginX + Mathf.Ceil((generationEntry.position.x - gridOriginX) / regionWidth) * regionWidth;

        nextRegionStartX = firstRegionX;
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
            Transform oldest = activeRegions.Peek();

            float regionEndX = oldest.position.x + regionWidth;

            if (regionEndX > cleanupBoundary)
                break;
            activeRegions.Dequeue();

            //The region contains no gameplay objects yet.
            oldest.gameObject.SetActive(false);
            availableRegions.Push(oldest);
        }
    }

    private void EnsureCoverage()
    {
        float requiredRightEdge = GetCameraRightEdge() + aheadBuffer;

        while (nextRegionStartX < requiredRightEdge)
        {
            Transform region = AcquireRegion();

            region.position = new Vector3(nextRegionStartX, generatedRegionsRoot.position.y, generatedRegionsRoot.position.z);
            activeRegions.Enqueue(region);

            nextRegionStartX += regionWidth;
        }
    }

    private Transform AcquireRegion()
    {
        if (availableRegions.Count > 0)
            return availableRegions.Pop();

        GameObject region = new GameObject($"GameplayRegion_{nextRegionId++}");

        Transform regionTransform = region.transform;
        regionTransform.SetParent(generatedRegionsRoot, false);

        return regionTransform;
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
        foreach (Transform region in activeRegions)
        {
            Vector3 center = new Vector3(region.position.x + regionWidth * 0.5f, region.position.y, region.position.z);
            Gizmos.DrawWireCube(center, new Vector3(regionWidth, regionHeight, 0f));
        }
    }
}