using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerMS : MonoBehaviour
{
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Transform gameHolder;

    private List<Tile> tiles = new();

    private int width;
    private int height;
    private int numMines;

    private readonly float tileSize = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        CreateGameBoard(9, 9, 10); // Easy
        // CreateGameBoard(16, 16, 40); // Intermediate
        // CreateGameBoard(30, 16, 99); // Expert
        ResetGameState();
    }



    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.TryGetComponent<Tile>(out Tile tile))
                {
                    if (tile.isMine)
                    {
                        GameOver();
                    }
                    else if (tile.active) 
                    {
                        tile.ClickTile();
                    } else // clicking a number
                    {
                        ExpandIfFlagged(tile);
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.TryGetComponent<Tile>(out Tile tile))
                {
                    tile.FlagTile();
                }
            }
        }

           
    }

    public void CreateGameBoard(int width, int height, int numMines)
    {
        // Save the game parameters we're using.
        this.width = width;
        this.height = height;
        this.numMines = numMines;

        // Create the array of tiles.
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                // Position the tile in the correct place (centred).
                Transform tileTransform = Instantiate(tilePrefab);
                tileTransform.parent = gameHolder;
                float xIndex = col - ((width - 1) / 2.0f);
                float yIndex = row - ((height - 1) / 2.0f);
                tileTransform.localPosition = new Vector3(xIndex * tileSize*2, yIndex * tileSize*2, 0);
                // Keep a reference to the tile for setting up the game.
                Tile tile = tileTransform.GetComponent<Tile>();
                tiles.Add(tile);
                tile.gameManagerMS = this;
            }
        }
    }

    private void ResetGameState()
    {
        // Randomly shuffle the tile positions to get indices for mine positions.
        int[] minePositions = Enumerable.Range(0, tiles.Count).OrderBy(x => Random.Range(0.0f, 1.0f)).ToArray();

        // Set mines at the first numMines positions.
        for (int i = 0; i < numMines; i++)
        {
            int pos = minePositions[i];
            tiles[pos].isMine = true;
        }

        // Update all the tiles to hold the correct number of mines.
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].mineCount = HowManyMines(i);
        }
    }

    // Given a location work out how many mines are surrounding it.
    private int HowManyMines(int location)
    {
        int count = 0;
        foreach (int pos in GetNeighbours(location))
        {
            if (tiles[pos].isMine)
            {
                count++;
            }
        }
        return count;
    }

    // Given a position, return the positions of all neighbours.
    public List<int> GetNeighbours(int pos)
    {
        List<int> neighbours = new();
        int row = pos / width;
        int col = pos % width;
        // (0,0) is bottom left.
        if (row < (height - 1))
        {
            neighbours.Add(pos + width); // North
            if (col > 0)
            {
                neighbours.Add(pos + width - 1); // North-West
            }
            if (col < (width - 1))
            {
                neighbours.Add(pos + width + 1); // North-East
            }
        }
        if (col > 0)
        {
            neighbours.Add(pos - 1); // West
        }
        if (col < (width - 1))
        {
            neighbours.Add(pos + 1); // East
        }
        if (row > 0)
        {
            neighbours.Add(pos - width); // South
            if (col > 0)
            {
                neighbours.Add(pos - width - 1); // South-West
            }
            if (col < (width - 1))
            {
                neighbours.Add(pos - width + 1); // South-East
            }
        }
        return neighbours;
    }

    public void ClickNeighbours(Tile tile)
    {
        int location = tiles.IndexOf(tile);
        foreach (int pos in GetNeighbours(location))
        {
            tiles[pos].ClickTile();
        }
    }

    public void GameOver()
    {
        // Disable clicks on all mines.
        foreach (Tile tile in tiles)
        {
            tile.ShowGameOverState();
        }
    }

    public void CheckGameOver()
    {
        // If there are numMines left active then we're done.
        int count = 0;
        foreach (Tile tile in tiles)
        {
            if (tile.active)
            {
                count++;
            }
        }
        if (count == numMines)
        {
            // Flag and disable everything, we're done.
            Debug.Log("Winner!");
            foreach (Tile tile in tiles)
            {
                tile.active = false;
                tile.SetFlaggedIfMine();
            }
        }
    }

    // Click on all surrounding tiles if mines are all flagged.
    public void ExpandIfFlagged(Tile tile)
    {
        int location = tiles.IndexOf(tile);
        // Get the number of flags.
        int flag_count = 0;
        foreach (int pos in GetNeighbours(location))
        {
            if (tiles[pos].flagged)
            {
                flag_count++;
            }
        }
        // If we have the right number click surrounding tiles.
        if (flag_count == tile.mineCount)
        {
            // Clicking a flag does nothing so this is safe.
            ClickNeighbours(tile);
        }
    }
}