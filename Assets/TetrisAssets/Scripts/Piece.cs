using System;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public TetrisGameManager gameBoard { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;


    public void Initialize(TetrisGameManager gameBoard, Vector3Int position, TetrominoData data)
    {
        this.gameBoard = gameBoard;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;
        this.stepTime = Time.time + stepDelay;
        this.lockTime = 0f;


        if (this.cells == null) this.cells = new Vector3Int[data.cells.Length];

        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        this.gameBoard.Clear(this);

        this.lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q)) Rotate(-1);
        if (Input.GetKeyDown(KeyCode.E)) Rotate(1);
        if (Input.GetKeyDown(KeyCode.A)) Move(Vector2Int.left);
        else if (Input.GetKeyDown(KeyCode.D)) Move(Vector2Int.right);
        if (Input.GetKeyDown(KeyCode.S)) Move(Vector2Int.down);
        if (Input.GetKeyDown(KeyCode.Space)) HardDrop();

        if(Time.time >= this.stepTime)
        {
            Step();
        }
        this.gameBoard.Set(this);
    }

    public void DecreaseStepDelay()
    {
        this.stepDelay = stepDelay - ((stepDelay * ((float)Math.Round(gameBoard.score, 0) / 100)) / 100) * 2;
    }

    private void Step()
    {
        this.stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);
        if(this.lockTime >= this.lockDelay)
        {
            LockPiece();
        }
    }

    private void LockPiece()
    {
        this.gameBoard.Set(this);
        this.gameBoard.ClearLines();
        this.gameBoard.SpawnPiece();
    }
    private void HardDrop()
    {
        while (Move(Vector2Int.down)) continue;
        LockPiece();
    }
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool isValid = this.gameBoard.IsValidPosition(this, newPosition);
        if (isValid) 
        {
            this.position = newPosition;
            //Refactor this so that the lockTime does not reset upon EVERY movement
            this.lockTime = 0f;
        }

        return isValid;
    }

    private void Rotate(int direction )
    {
        int originalRotation = this.rotationIndex;
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

        ApplyRotationMatrix(direction);

        if (!TestWallKicks(rotationIndex, direction))
        {
            this.rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }

    }

    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];
            int x, y;
            switch (this.data.tetromino)
            {
                case Tetromino.I:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * TetrominoRotationData.RotationMatrix[0] * direction) + (cell.y * TetrominoRotationData.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * TetrominoRotationData.RotationMatrix[2] * direction) + (cell.y * TetrominoRotationData.RotationMatrix[3] * direction));
                    break;
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * TetrominoRotationData.RotationMatrix[0] * direction) + (cell.y * TetrominoRotationData.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * TetrominoRotationData.RotationMatrix[2] * direction) + (cell.y * TetrominoRotationData.RotationMatrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * TetrominoRotationData.RotationMatrix[0] * direction) + (cell.y * TetrominoRotationData.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * TetrominoRotationData.RotationMatrix[2] * direction) + (cell.y * TetrominoRotationData.RotationMatrix[3] * direction));
                    break;
            }
            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);
        for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
           
            Vector3 translation = new Vector3(this.data.wallKicks[wallKickIndex, i].x, this.data.wallKicks[wallKickIndex, i].y, 0).normalized;
            Vector2Int translationNormalized = new Vector2Int((int)translation.x, (int)translation.y);
            if (Move(translationNormalized))
            {
                return true;
            }
        }
        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickindex = rotationIndex * 2; 
        if(rotationDirection < 0)
        {
            wallKickindex--;
        }

        return Wrap(wallKickindex, 0, this.data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min) return max - (min - input) % (max - min);
        else return min + (input - min) % (max - min);
    }
}
