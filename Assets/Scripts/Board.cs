using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition= new Vector3Int(-1,8,0);
    public Vector2Int boardSize=new Vector2Int(10,20);

    public RectInt Bounds
    {
        get 
        {
            Vector2Int position= new Vector2Int(-this.boardSize.x/2,-this.boardSize.y/2);
            return new RectInt(position, this.boardSize);
        }
    }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece=GetComponentInChildren<Piece>();
        for(int i=0;i<this.tetrominos.Length;i++)
        {
            this.tetrominos[i].Initialize();
        }
    }
    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        int random=Random.Range(0,this.tetrominos.Length);
        TetrominoData data = this.tetrominos[(int)random];
        activePiece.Initialize(this,spawnPosition,data);
        if(IsValidPosition(activePiece,spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }
    public void GameOver()
    {
        tilemap.ClearAllTiles();
    }
    public void Set(Piece piece)
    {
        for(int i=0;i<piece.cells.Length;i++)
        {
            Vector3Int tilePosition = piece.cells[i]+piece.position;
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
        RectInt bounds = this.Bounds;
        for(int i=0;i<piece.cells.Length;i++) 
        {
            Vector3Int tilePosition= piece.cells[i]+position;
            if(!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
            if(this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }
        return true;
    }
    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        while(row<bounds.yMax)
        {
            if(IsLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }
    }
    public bool IsLineFull(int row)
    {
        RectInt bounds=Bounds;
        for(int col=bounds.xMin;col<bounds.xMax;col++)
        {
            Vector3Int position= new Vector3Int(col,row,0);
            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }
        return true;
    }
    public void LineClear(int row)
    {
        RectInt bounds = Bounds;
        for(int col=bounds.xMin;col< bounds.xMax;col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }
        while(row<bounds.yMax)
        {
            for(int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col,row+1,0);
                TileBase above=tilemap.GetTile(position);
                position=new Vector3Int(col,row,0);
                tilemap.SetTile(position, above);
            }
            row++;
        }
    }
}
