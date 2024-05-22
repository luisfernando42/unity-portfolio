using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public TetrisGameManager gameBoard;
    public Piece trackingPiece;

    public Tilemap ghostTileMap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private void Awake()
    {
        this.ghostTileMap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.ghostTileMap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = this.trackingPiece.cells[i];
        }
    }

    private void Drop()
    {
        Vector3Int position = this.trackingPiece.position;

        int currentRow = position.y;
        int gameBoardBottom = -this.gameBoard.boardSize.y / 2 - 1;

        this.gameBoard.Clear(this.trackingPiece);
        for (int row = currentRow; row >= gameBoardBottom; row--)
        {
            position.y = row;
            if(this.gameBoard.IsValidPosition(this.trackingPiece, position))
            {
                this.position = position;
            } 
            else
            {
                break;
            }
        }
        this.gameBoard.Set(this.trackingPiece);
    }
    
    private void Set()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.ghostTileMap.SetTile(tilePosition, this.tile);
        }
    }


}
