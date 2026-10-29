using System;
using UnityEngine;
using Random = UnityEngine.Random;


public sealed class InfiniteBackgroundStrip : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;

    [SerializeField] private Transform[] chunks;

    [Header("Chunk Settings")]
    [SerializeField][Min(0.01f)] private float chunkLength = 16f;

    private void LateUpdate()
    {
        RecyclePassedChunks();
    }

    private void RecyclePassedChunks()
    {
        if (targetCamera == null)
            return;
        if (chunks == null || chunks.Length == 0)
            return;
        float cameraLeftEdge = GetCameraLeftEdge();
        bool recycled;

        do
        {
            recycled = false;
            Transform randomChunk = FindRandomChunkX(chunks);

            float chunkRightEdge = randomChunk.position.x + chunkLength * 0.5f;

            if (chunkRightEdge >= cameraLeftEdge)
                continue;

            float rightmostX = FindRightmostChunkX();

            Vector3 position = randomChunk.position;

            position.x = rightmostX + chunkLength;

            randomChunk.position = position;

            recycled = true;
        } while (recycled);
    }

    private float GetCameraLeftEdge()
    {
        Vector3 leftEdge = targetCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, Mathf.Abs(targetCamera.transform.position.z)));
        return leftEdge.x;
    }

    private int FindLeftmostChunkIndex()
    {
        int result = 0;
        float leftmostX = chunks[0].position.x;

        for (int i = 1; i < chunks.Length; i++)
        {
            if (chunks[i].position.x >= leftmostX)
                continue;
            leftmostX = chunks[i].position.x;
            result = i;
        }

        return result;
    }

    private float FindRightmostChunkX()
    {
        float rightmostX =
            chunks[0].position.x;

        for (int i = 1; i < chunks.Length; i++)
        {
            if (chunks[i].position.x <= rightmostX)
                continue;

            rightmostX =
                chunks[i].position.x;
        }

        return rightmostX;
    }

    private Transform FindRandomChunkX(Transform[] chunks)
    {
        if (chunks.Length == 0) {  return null; }

        bool keepSearching = true;
        Transform randomTransform;

        do 
        {
            randomTransform = chunks[Random.Range(0, chunks.Length)];
            float rightmostChunk = FindRightmostChunkX();

            if (randomTransform.position.x != rightmostChunk && randomTransform.position.x != rightmostChunk - 16f)
            {
                keepSearching = false;
                return randomTransform;
            }

        } while (keepSearching);

        return randomTransform;
    }
}
