using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TetrisGrid),typeof(TetrisSpawner), typeof(TetrisSpawner))]
public class TetrisGame : MonoBehaviour
{
    public float fallSpeed = 1f;
    public float verticalMoveSpeed = 0.05f;
    public float horizontalMoveSpeed = 0.05f;
    public float rotateSpeed = 0.05f;

    public GameObject activePiece { get; private set; } = null;

    public TetrisSpawner spawner { get; private set; }
    public TetrisGrid grid { get; private set; }
    public TetrisLineClearRenderer tetrisLineClearRenderer { get; private set; }

    private void Awake()
    {
        grid = GetComponent<TetrisGrid>();
        spawner = GetComponent<TetrisSpawner>();
        tetrisLineClearRenderer = GetComponent<TetrisLineClearRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        spawner.InstantSpawn();
    }

    public void OnPlant(TetrisPiece piece)
    {
        TetrisLineClearData lineClearData = grid.ClearLines();
        if (lineClearData.lineClearCount > 0)
            tetrisLineClearRenderer.Render(lineClearData);
        else
            OnContinue();
    }

    public void OnContinue()
    {
        spawner.InstantSpawn();
    }

    public void OnTetrisHold(TetrisHold tetrisHold)
    {
        Destroy(activePiece);
        spawner.InstantSpawn(tetrisHold.previousHeldPiece);
    }

    public void SetActivePiece(GameObject piece)
    {
        activePiece = piece;
    }
}
