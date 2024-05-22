
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TetrisGameManager : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominoes;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);


    public float score { get; private set; } = 0;
    private float rowClearAmount = 0;
    private float rowclearTime = 0f;
    private float rowClearDelay = 1f;

    [SerializeField] private TextMeshProUGUI scoreTMP;
    

    public RectInt bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }

    private void Awake()
    {
        InitializeTetrominoes();
    }

    void InitializeTetrominoes()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < this.tetrominoes.Length; i++)
        {
            this.tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, this.tetrominoes.Length);
        TetrominoData data = this.tetrominoes[random];

        this.activePiece.Initialize(this, this.spawnPosition, data);

        if(IsValidPosition(this.activePiece, this.spawnPosition))
        {
            Set(this.activePiece);
        }
        else
        {
            GameOver();
        }

    }

    private void Update()
    {
        rowclearTime += Time.deltaTime;

        if(rowclearTime >= rowClearDelay)
        {
            rowClearAmount = 0;
        }
    }

    private void GameOver()
    {

        this.tilemap.ClearAllTiles();
        //Add helper to SceneManager
        SceneManager.LoadScene("Menu");
      
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.bounds;
        for (int i = 0; i < piece.cells.Length; i++)
        {

            Vector3Int tilePosition = piece.cells[i] + position;
            if (!bounds.Contains((Vector2Int)tilePosition)) return false;
            if (this.tilemap.HasTile(tilePosition)) return false;
        }
        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = this.bounds;
        int row = bounds.yMin;
        while (row < bounds.yMax)
        {
            if (IsLineFull(row)) 
            { 
                LineClear(row);
                score += 100 + rowClearAmount * 10;
                scoreTMP.text = score.ToString();
            }
            else row++;
        }
    }

    private void LineClear(int row)
    {
        RectInt bounds = this.bounds;
        for (int column = bounds.xMin; column < bounds.xMax; column++)
        {
            Vector3Int position = new Vector3Int(column, row, 0);
            this.tilemap.SetTile(position, null);
        }
        while (row < bounds.yMax)
        {
            for (int column = bounds.xMin; column < bounds.xMax; column++)
            {
                Vector3Int position = new Vector3Int(column, row + 1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position = new Vector3Int(column, row, 0);
                this.tilemap.SetTile(position, above);
            }
            row++;
        }
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = this.bounds;
        for (int column = bounds.xMin; column < bounds.xMax; column++)
        {
            Vector3Int position = new Vector3Int(column, row, 0);
            if(!this.tilemap.HasTile(position)) return false;
        }
        rowclearTime = 0f;
        rowClearAmount++;
        return true;
    }

}
