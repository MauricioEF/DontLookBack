using System.Collections.Generic;
using UnityEngine;

public sealed class GameplayRegion : MonoBehaviour
{
    private readonly List<GameObject> spawnedObjects = new();

    public GenerationPattern CurrentPattern
    {
        get;
        private set;
    }

    public void Prepare(GenerationPattern pattern)
    {
        Clear();

        if (pattern == null)
        {
            Debug.LogWarning($"Cannot prepare a region without a pattern", this);
            return;
        }

        CurrentPattern = pattern;

        foreach (PatternObjectPlacement placement in pattern.Objects)
        {
            if (placement.Prefab == null)
                continue;
            Vector2 localPosition = placement.LocalPosition;

            GameObject instance = Instantiate(placement.Prefab, transform);

            instance.transform.localPosition = new Vector3(localPosition.x, localPosition.y, 0f);

            spawnedObjects.Add(instance);
        }
    }

    public void Clear()
    {
        foreach (GameObject instance in spawnedObjects)
        {
            if (instance == null)
                continue;
            instance.SetActive(false);
            Destroy(instance);
        }
        spawnedObjects.Clear();
        CurrentPattern = null;
    }
}
