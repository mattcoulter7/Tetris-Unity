using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisLineClearData
{
    public Dictionary<Transform, Vector2Int> newPositions { get; private set; } = new Dictionary<Transform, Vector2Int>();
    public List<Transform> clearedTransforms { get; private set; } = new List<Transform>();
    public List<int> clearedLineIndices { get; private set; } = new List<int>();
    public int lineClearCount = 0;
}