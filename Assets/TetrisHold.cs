using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TetrisHold : MonoBehaviour
{
    public string previousHeldPiece = null;
    public string heldPiece = null;
    private PlayerInput playerInput;
    private TetrisGame tetrisGame;
    private void Awake()
    {
        tetrisGame = GetComponent<TetrisGame>();
    }
    private void OnEnable()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions["Hold"].performed += OnHold;
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Hold"].performed -= OnHold;
        }
    }

    /// <returns>The Previous piece that was held</returns>
    public void Hold()
    {
        previousHeldPiece = heldPiece;
        heldPiece = tetrisGame.activePiece.name;

        BroadcastMessage("OnTetrisHold", this);
    }

    private void OnHold(InputAction.CallbackContext ctx)
    {
        Hold();
    }
}
