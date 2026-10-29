using System.Collections.Generic;
using UnityEngine;

public sealed class GenerationPatternSelector : MonoBehaviour
{
    [Header("Pattern Sequence")]

    [SerializeField] private List<GenerationPattern> patterns = new();

    private int nextPatternIndex;

    public bool HasValidPatterns
    {
        get
        {
            if (patterns == null || patterns.Count == 0)
                return false;
            foreach (GenerationPattern pattern in patterns)
            {
                if (pattern == null)
                    return false;
            }
            return true;
        }
    }

    public void ResetSequence()
    {
        nextPatternIndex = 0;
    }

    public bool TrySelectNext(out GenerationPattern pattern)
    {
        pattern = null;

        if (!HasValidPatterns)
            return false;

        pattern = patterns[nextPatternIndex];
        nextPatternIndex = (nextPatternIndex + 1) % patterns.Count;
        return true;
    }
}
