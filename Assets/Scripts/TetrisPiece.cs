using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;
using System;

public class TetrisPiece : MonoBehaviour
{
    public Transform rotationOrigin;

    public bool isIPiece = false;
    public GameObject UIPrefab;

    private float lastFallTime;
    private float lastHorizontalMoveTime;
    private float lastRotateTime;

    private PlayerInput playerInput;
    private HeldPlayerInput heldPlayerInput;

    private TetrisGame tetrisGame = null;
    private PlantTimer plantTimer = null;

    private void Awake()
    {
        // find input systems
        playerInput = FindObjectOfType<PlayerInput>();
        heldPlayerInput = FindObjectOfType<HeldPlayerInput>();

        // find tetris game instance
        tetrisGame = GetComponentInParent<TetrisGame>();
        tetrisGame.SetActivePiece(gameObject);

        // initialise the timer used for managing when the piece is finally 'planted'
        plantTimer = new PlantTimer();
        plantTimer.onTimerFinish += Plant;
    }

    private void OnEnable()
    {
        // bind controls
        playerInput.actions["HorizontalStep"].performed += OnHorizontalStep;
        playerInput.actions["RotateStep"].performed += OnRotateStep;
        playerInput.actions["HardDrop"].performed += OnHardDrop;
        heldPlayerInput.performed["HorizontalHold"] += OnHorizontalHold;
        heldPlayerInput.performed["RotateHold"] += OnRotateHold;
    }
    private void OnDisable()
    {
        // unbind controls
        if (playerInput != null)
        {
            playerInput.actions["HorizontalStep"].performed -= OnHorizontalStep;
            playerInput.actions["RotateStep"].performed -= OnRotateStep;
            playerInput.actions["HardDrop"].performed -= OnHardDrop;
        }
        if (heldPlayerInput != null)
        {
            heldPlayerInput.performed["HorizontalHold"] -= OnHorizontalHold;
            heldPlayerInput.performed["RotateHold"] -= OnRotateHold;
        }
    }
    private void Update()
    {
        // handle moving down
        float targetFallSpeed = playerInput.actions["Fall"].ReadValue<float>() != 0f ? tetrisGame.verticalMoveSpeed : tetrisGame.fallSpeed;
        if (Time.time - lastFallTime > targetFallSpeed)
        {
            MoveDown();
        }
    }

    /// <summary>
    /// Moves the pieces down 1 if valid
    /// </summary>
    private void MoveDown()
    {
        if (!ValidMove(-Vector3.up))
        {
            // start the plant timer to plant after x given seconds/rest for x given seconds
            plantTimer.Start(this);
            return;
        }

        transform.position -= Vector3.up;
        lastFallTime = Time.time;

        // Stop the plant timer as they were able to move down
        plantTimer.Stop(this);

        // need to start the plant timer automatically after the touch the ground
        if (!ValidMove(-Vector3.up))
        {
            plantTimer.Start(this);
            return;
        }
    }

    /// <summary>
    /// Moves the pieces horizontally in a given direction
    /// </summary>
    /// <param name="dir">Positive: Right, Negative: left, 0: Don't move</param>
    private void MoveHorizontal(float dir)
    {
        if (dir == 0) return;

        Vector3 moveAmount = dir > 0 ? Vector3.right : -Vector3.right;
        if (!ValidMove(moveAmount))
        {
            return;
        }

        transform.position += moveAmount;
        lastHorizontalMoveTime = Time.time;

        if (!ValidMove(-Vector3.up)) plantTimer.RestartRest(this);
    }

    /// <summary>
    /// Rotate the pieces in a given direction
    /// </summary>
    /// <param name="dir">Positive: CounterClockwise, Negative: Clockwise, 0: Don't rotate</param>
    private void Rotate(float dir,bool wallKick = false)
    {
        if (dir == 0) return;

        float rotationAmount = dir > 0 ? -90 : 90;
        if (!ValidMove(null, rotationAmount))
        {
            if (wallKick) WallKick(dir);
            return;
        }

        transform.RotateAround(rotationOrigin.position, new Vector3(0, 0, 1), rotationAmount);

        lastRotateTime = Time.time;

        if (!ValidMove(-Vector3.up)) plantTimer.RestartRest(this);
    }
    private bool Move(Vector2Int offset, float rotationDir = 0)
    {
        float rotationAmount = rotationDir == 0 ? 0 : rotationDir > 0 ? -90 : 90;

        if (!ValidMove(offset, rotationAmount)) return false;
        transform.RotateAround(rotationOrigin.position, new Vector3(0, 0, 1), rotationAmount);
        transform.position += new Vector3(offset.x, offset.y);
        return true;
    }

    /// <summary>
    /// A rotation wasn't able to happen, so we can help the player by moving the piece over by 1 in either direction then perform the rotation
    /// </summary>
    private void WallKick(float dir)
    {
        if (dir == 0) return;

        int rotationIndex = GetRotationIndex();
        int wallKickIndex = TetrisWallKicks.GetWallKickIndex(rotationIndex, dir);

        for (int i = 0; i < TetrisWallKicks.WallKickCount; i++)
        {
            try
            {
                Vector2Int offset = TetrisWallKicks.WallKicks[isIPiece][wallKickIndex, i];
                if (Move(offset, dir))
                {

                    return;
                }
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }

        }
    }

    public int GetRotationIndex()
    {
        return Mathf.RoundToInt(Wrap((int)-transform.eulerAngles.z,0,360) / 90) % 4;
    }

    public List<Transform> GetPieces()
    {
        List<Transform> pieces = new List<Transform>();
        foreach (Transform piece in transform)
        {
            if (piece.tag != "TetrisPieceCube") continue;
            pieces.Add(piece);
        }
        return pieces;
    }

    public Rect GetRect()
    {
        List<Transform> pieces = GetPieces();
        IEnumerable<float> xPositions = pieces.Select(p => p.position.x);
        IEnumerable<float> yPositions = pieces.Select(p => p.position.y);
        float minX = xPositions.Min();
        float minY = yPositions.Min();
        float maxX = xPositions.Max();
        float maxY = yPositions.Max();

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    /// <summary>
    /// Determine whether a move is valid before or after transformation.
    /// If both parameters are null, it will determine if the current position is valid
    /// </summary>
    /// <param name="offset">The position delta</param>
    /// <param name="rotation">The rotation delta</param>
    public bool ValidMove(Vector2? offset = null,float rotation = 0)
    {
        if (tetrisGame == null) return true;

        List<Transform> pieces = GetPieces();
        if (pieces.Count == 0) return false;

        foreach (Transform piece in pieces)
        {
            Vector2 pos = piece.transform.position;
            Vector2 offsetRotationOrigin = rotationOrigin.position;
            if (offset.HasValue)
            {
                pos += offset.Value;
                offsetRotationOrigin += offset.Value;
            }

            pos = pos.Rotate(rotation, offsetRotationOrigin);

            if (!tetrisGame.grid.PointInGrid(pos)) return false;
            if (tetrisGame.grid.PointTaken(pos)) return false;
        }
        return true;
    }

    /// <summary>
    /// Called when the piece is finally placed
    /// </summary>
    private void Plant()
    {
        if (tetrisGame == null) return;

        if (ValidMove(-Vector3.up)) return;

        foreach (Transform piece in GetPieces())
        {
            tetrisGame.grid.SaveToGrid(piece);
        }
        StopAllCoroutines();
        enabled = false;

        SendMessageUpwards("OnPlant",this);
        BroadcastMessage("OnPlant", this);
    }

    private void HardDrop()
    {
        int shadowDistance = GetShadowDistance();
        Vector3 shadowDelta = shadowDistance * Vector3.up;

        transform.position -= shadowDelta;

        Plant();
    }

    public int GetShadowDistance()
    {
        int i = 1;
        while (ValidMove(-Vector3.up * i))
        {
            i++;
        }
        return i - 1;
    }

    private void OnHorizontalStep(InputAction.CallbackContext ctx)
    {
        float dir = ctx.ReadValue<float>();
        MoveHorizontal(dir);
    }
    private void OnHorizontalHold(InputAction.CallbackContext ctx)
    {
        float dir = ctx.ReadValue<float>();
        if (Time.time - lastHorizontalMoveTime > tetrisGame.horizontalMoveSpeed)
            MoveHorizontal(dir);
    }
    private void OnRotateStep(InputAction.CallbackContext ctx)
    {
        float dir = ctx.ReadValue<float>();
        Rotate(dir,true);
    }
    private void OnRotateHold(InputAction.CallbackContext ctx)
    {
        float dir = ctx.ReadValue<float>();
        if (Time.time - lastRotateTime > tetrisGame.rotateSpeed)
            Rotate(dir,true);
    }
    private void OnHardDrop(InputAction.CallbackContext ctx)
    {
        HardDrop();
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }
}
