using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    public Transform bounds;

    public Transform[,] grid;

    public int width { get; private set; }
    public int height { get; private set; }

    private Vector2 gridOffset; // how far does the grid need to move to get to 0,0

    private TetrisLineClearData lineClearData;

    // Start is called before the first frame update
    void Start()
    {
        width = (int)bounds.localScale.x;
        height = (int)bounds.localScale.y;

        grid = new Transform[width, height];
        gridOffset = Vector2.zero - GetBottomLeft();
    }

    public void SaveToGrid(Transform t)
    {
        Vector2Int gridPoint = WorldPointToGridPoint(t.position);
        if (!PointInGrid(gridPoint)) return;
        grid[gridPoint.x, gridPoint.y] = t;
    }

    public bool PointInGrid(Vector2 point)
    {
        Vector2 gridBL = GetBottomLeft();
        Vector2 gridTR = GetTopRight();
        return point.x >= gridBL.x && point.x <= gridTR.x && point.y >= gridBL.y;
    }
    public bool PointInGrid(Vector2Int point)
    {
        return point.x >= 0 && point.x < width && point.y >= 0 && point.y < height;
    }

    public Vector2 TransformPoint(Vector2 pt)
    {
        return pt + gridOffset;
    }

    public Vector2 InverseTransformPoint(Vector2 pt)
    {
        return pt - gridOffset;
    }

    public Vector2Int WorldPointToGridPoint(Vector2 pt)
    {
        pt = TransformPoint(pt);
        return new Vector2Int(Mathf.FloorToInt(pt.x), Mathf.FloorToInt(pt.y));
    }
    public Vector2 GridPointToWorldPoint(Vector2Int pt)
    {
        Vector2 worldPt = InverseTransformPoint(pt);
        worldPt += new Vector2(0.5f, 0.25f);
        return worldPt;
    }

    public bool PointTaken(Vector2 pt)
    {
        Vector2Int gridPoint = WorldPointToGridPoint(pt);
        try
        {
            return grid[gridPoint.x, gridPoint.y] != null;
        }
        catch(IndexOutOfRangeException e)
        {
            return false;
        }
    }
    public Vector2 GetBottomLeft()
    {
        return bounds.transform.position - new Vector3(width / 2, height / 2);
    }
    public Vector2 GetTopRight()
    {
        return bounds.transform.position + new Vector3(width / 2, height / 2);
    }

    public TetrisLineClearData ClearLines()
    {
        lineClearData = new TetrisLineClearData();

        for (int i = 0; i < height; i++)
        {
            if (HasLine(i))
            {
                ClearLine(i);
                DropLines(i);
                lineClearData.lineClearCount++;
                i--;
            }
        }

        return lineClearData;
    }

    private bool HasLine(int rowIndex)
    {
        for (int colIndex = 0; colIndex < width; colIndex++)
        {
            if (grid[colIndex, rowIndex] == null) return false;
        }
        return true;
    }

    private void ClearLine(int rowIndex)
    {
        for (int colIndex = 0; colIndex < width; colIndex++)
        {
            Transform piece = grid[colIndex, rowIndex];
            if (piece == null) continue;
            grid[colIndex, rowIndex] = null;
            lineClearData.clearedTransforms.Add(piece);

            if (lineClearData.newPositions.ContainsKey(piece))
            {
                lineClearData.newPositions.Remove(piece);
            }
        }
    }
    private void DropLines(int rowIndex)
    {
        for (int dropLineIndex = rowIndex + 1; dropLineIndex < height; dropLineIndex++)
        {
            for (int colIndex = 0; colIndex < width; colIndex++)
            {
                if (grid[colIndex, dropLineIndex] != null)
                {
                    grid[colIndex, dropLineIndex - 1] = grid[colIndex, dropLineIndex];
                    grid[colIndex, dropLineIndex] = null;
                    lineClearData.newPositions[grid[colIndex, dropLineIndex - 1]] = new Vector2Int(colIndex, dropLineIndex - 1);
                }
            }
        }
    }
}
