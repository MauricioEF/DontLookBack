using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationPattern", menuName = "Scriptable Objects/GenerationPattern")]
public class GenerationPattern : ScriptableObject
{
    [SerializeField] private string patternId;

    [SerializeField] private List<PatternObjectPlacement> objects = new();

    public string PatternId => patternId;
    public IReadOnlyList<PatternObjectPlacement> Objects => objects;
}

[Serializable]
public sealed class PatternObjectPlacement
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector2 localPosition;

    public GameObject Prefab => prefab;
    public Vector2 LocalPosition => localPosition;
}
