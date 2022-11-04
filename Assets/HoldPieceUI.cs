using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldPieceUI : MonoBehaviour
{
    public Transform targetPosition;

    private GameObject currentUIInstance = null;
    public void OnTetrisHold(TetrisHold tetrisHold)
    {
        if (currentUIInstance) Destroy(currentUIInstance);

        GameObject tetrisUIPrefab = Resources.Load<GameObject>($"Prefabs/{tetrisHold.heldPiece}-UI");
        currentUIInstance = Instantiate(tetrisUIPrefab, targetPosition);
    }
}
