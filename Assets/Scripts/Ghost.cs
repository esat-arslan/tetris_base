using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile; // "The tile used for rendering the ghost piece"
    public Board mainBoard; //" Reference to the game board"
    public Piece trackingPiece; //"The tetromino being tracked by the ghost piece"

    public Tilemap tilemap { get; private set; } //"Reference to the Tilemap component"
    public Vector3Int[] cells { get; private set; } //"Individual cells of the ghost piece"
    public Vector3Int position { get; private set; } //"Position of the ghost piece"

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>(); //"Get the Tilemap component from child objects"
        cells = new Vector3Int[4]; //"Initialize the cells array with a size of 4"
    }

    private void LateUpdate()
    {
        Clear(); //"Clear the ghost piece from the Tilemap"
        Copy(); //"Copy the cells of the tracking piece"
        Drop(); //"Update the position of the ghost piece"
        Set(); //"Render the ghost piece on the Tilemap"
    }

    private void Clear()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position; //"Calculate the position of the tile to clear"
            tilemap.SetTile(tilePosition, null); //"Remove the tile at the specified position"
        }
    }

    private void Copy()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = trackingPiece.cells[i]; //"Copy the cells of the tracking piece to the ghost piece"
        }
    }

    private void Drop()
    {
        Vector3Int position = trackingPiece.position; //"Get the position of the tracking piece"

        int current = position.y; //" Current row position"
        int bottom = -mainBoard.boardSize.y / 2 - 1; //"Bottom row position of the game board"

        mainBoard.Clear(trackingPiece); //"Clear the tracking piece from the game board"

        for (int row = current; row >= bottom; row--)
        {
            position.y = row; //"Update the position to the current row"

            if (mainBoard.IsValidPosition(trackingPiece, position))
            {
                this.position = position; //"Update the position of the ghost piece"
            }
            else
            {
                break; //"Stop dropping if an invalid position is encountered"
            }
        }

        mainBoard.Set(trackingPiece); //"Set the tracking piece back to the game board"
    }

    private void Set()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position; //"Calculate the position to render the tile"
            tilemap.SetTile(tilePosition, tile); //"Render the tile at the specified position"
        }
    }

}
