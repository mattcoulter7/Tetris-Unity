using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisLineClearRenderer : MonoBehaviour
{
    public GameObject clearLineParticleSystem;

    public float lineClearDuration = 0.5f;
    public float lineDropDuration = 0.2f;
    private TetrisGame tetrisGame;
    private void Start()
    {
        tetrisGame = GetComponent<TetrisGame>();
    }
    public void Render(TetrisLineClearData lineClearData)
    {
        StartCoroutine(RenderProcess(lineClearData));
    }

    private IEnumerator RenderProcess(TetrisLineClearData lineClearData)
    {
        ClearLines(lineClearData);
        yield return new WaitForSeconds(lineClearDuration);
        DropLines(lineClearData);
        yield return new WaitForSeconds(lineDropDuration);
        ShiftLines(lineClearData);
        SendMessage("OnContinue");
    }

    private void ClearLines(TetrisLineClearData lineClearData)
    {
        // Clear the lines
        foreach (Transform piece in lineClearData.clearedTransforms)
        {
            Destroy(piece.gameObject);
        }

        foreach (int rowIndex in lineClearData.clearedLineIndices)
        {

        }
    }

    private void DropLines(TetrisLineClearData lineClearData)
    {
        foreach (Transform piece in lineClearData.newPositions.Keys)
        {
            Rigidbody rb = piece.gameObject.GetComponent<Rigidbody>();
            if (rb == null)
                rb = piece.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = false;
        }
    }

    private void ShiftLines(TetrisLineClearData lineClearData)
    {
        foreach (KeyValuePair<Transform, Vector2Int> pair in lineClearData.newPositions)
        {
            pair.Key.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            Vector3 targetPosition = tetrisGame.grid.GridPointToWorldPoint(pair.Value);
            pair.Key.position = targetPosition;
        }
    }
}
