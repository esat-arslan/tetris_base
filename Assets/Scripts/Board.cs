using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    //initiliaze board
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition= new Vector3Int(-1,8,0);
    public Vector2Int boardSize=new Vector2Int(10,20);

    public RectInt Bounds //"This is a read-only property 'Bounds' that returns the rectangular boundaries of the game board based on the 'boardSize' variable."
    {
        get 
        {
            Vector2Int position= new Vector2Int(-this.boardSize.x/2,-this.boardSize.y/2);
            return new RectInt(position, this.boardSize);
        }
    }

    private void Awake() //"This method is executed during the 'Awake' phase of the MonoBehaviour lifecycle. It initializes the 'tilemap' and 'activePiece' variables by getting the 'Tilemap' and 'Piece' components from the children objects. Additionally, it initializes each TetrominoData object in the tetrominos array by calling its Initialize() method."
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece=GetComponentInChildren<Piece>();
        for(int i=0;i<this.tetrominos.Length;i++)
        {
            this.tetrominos[i].Initialize();
        }
    }
    private void Start() //"This method is executed during the Start phase of the MonoBehaviour lifecycle. It calls the 'SpawnPiece()' method to start the game by spawning a new piece."
    {
        SpawnPiece();
    }

    public void SpawnPiece() //"This method is responsible for spawning a new piece in the game. It randomly selects a TetrominoData object from the tetrominos array and initializes the activePiece with the selected data at the spawnPosition. If the initial position of the piece is valid, it sets the tiles on the Tilemap using the Set() method. Otherwise, it triggers the GameOver() method."
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
    public void GameOver() //"This method is called when the game is over. It clears all the tiles on the Tilemap."
    {
        tilemap.ClearAllTiles();
    }
    public void Set(Piece piece) //"This method is responsible for setting the tiles on the Tilemap based on the cells of a given piece. It iterates through the cells of the piece, calculates the tile position by adding the cell position and the piece position, and sets the corresponding tile on the Tilemap using the SetTile()"
    {
        for(int i=0;i<piece.cells.Length;i++)
        {
            Vector3Int tilePosition = piece.cells[i]+piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece) //"This method is responsible for clearing the tiles on the Tilemap occupied by a given piece. It iterates through the cells of the piece, calculates the tile position by adding the cell position and the piece position, and sets the corresponding tile on the Tilemap to null, effectively clearing it."
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position) //"This method checks whether a given position for a piece is valid within the game board. It iterates through the cells of the piece, calculates the tile position by adding the cell position and the given position, and performs the following checks:

If the tile position is outside the board boundaries (bounds), it returns false.
If the Tilemap already has a tile at the tile position, it returns false.
If none of these conditions are met, it returns true, indicating that the position is valid."
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
    public void ClearLines()//"This method is responsible for clearing any full lines on the game board. It iterates through each row within the board boundaries and checks if the line is full using the IsLineFull() method. If a line is full, it clears the line using the LineClear() method. If the line is not full, it moves to the next row."
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
    public bool IsLineFull(int row)//"This method checks if a specific row on the game board is full, indicating that all cells in the row have tiles. It iterates through each column within the board boundaries and checks if each tile position in the row has a tile. If any position doesn't have a tile, it returns false, indicating that the line is not full. If all positions have tiles, it returns true, indicating that the line is full."
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
    public void LineClear(int row)//"This method is called when a line is full and needs to be cleared. It first clears all the tiles in the specified row by iterating through each column within the board boundaries and setting the tile at each position to `null`.
After clearing the line, it shifts all the rows above the cleared line down by one. It starts from the row above the cleared line and iterates through each row within the board boundaries. For each row, it iterates through each column and gets the tile from the position one row above. Then, it sets the tile at the current position to the retrieved tile, effectively shifting the tile down by one row. This process continues until all the rows above the cleared line have been shifted down."
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
