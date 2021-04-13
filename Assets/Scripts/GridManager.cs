using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;


public class GridManager : Manager
{
    Animator animator;
    public Canvas canvas;
    public GameObject prefab; 
    public List<Sprite> sprites = new List<Sprite>();
    public int gridWidth = 3;
    public float tileSize;
    public float tileGap;
    public List<HexTile> inventory = new List<HexTile>();
    public Dictionary<Vector3Int, HexTile> tiles = new Dictionary<Vector3Int, HexTile>();
    public HashSet<Vector3Int> tilesHash = new HashSet<Vector3Int>();
    public List<HexTile> activeTiles = new List<HexTile>();
    public Vector3 hoverTilePosition = Vector3.zero;
    public HexTile hoverTile;
    public HexTile oldHoverTile;
    public HexTile activeTile;
    public List<HexTile> completedTiles = new List<HexTile>();
    public GridState gridState;
    
    List<Vector3Int> tileDirections = new List<Vector3Int>  {
                                                                new Vector3Int(1, -1, 0), 
                                                                new Vector3Int(1, 0, -1), 
                                                                new Vector3Int(0, 1, -1), 
                                                                new Vector3Int(-1, 1, 0), 
                                                                new Vector3Int(-1, 0, 1), 
                                                                new Vector3Int(0, -1, 1)
                                                            };
    
    protected override void Awake()
    {
        base.Awake();
        world.gridManager = this;
        animator = GetComponent<Animator>();
    }
    protected override void Start()
    {
        base.Start();
        CreateTileMap();
        GetTile(Vector3Int.zero).Activate(TileState.Completed);

        for (int i = 0; i < 4; i++)
        {
            GetRandomTile(3).Activate();
        }

        // setup inventory
        foreach (HexTile tile in inventory)
        {
            tile.Init();
            tile.Activate(TileState.Inventory);
        }
    } 

    public void InPlacement(HexTile tile)
    {
        activeTile = tile;
        animator.SetBool("IsDraging", true);
    }

    public void InRotation(bool rotate = true)
    {
        animator.SetBool("IsRotating", rotate);
    }

    public void ButtonCompletePlacement()
    {
        if (TilePlacementValid(activeTile))
        {
            activeTile.ConfirmTilePlacement();
            animator.SetBool("IsComplete", true);
        }
        else
        {
            Debug.Log("Here?");
            animator.SetBool("IsRotating", false);
        }
    }

    public void ExitPlacement()
    {
        activeTile = null;
        oldHoverTile = null;
        animator.SetBool("IsDraging", false);
    }

    void CreateTileMap()
    {
        for (int q = -gridWidth; q <= gridWidth; q++)
        {
            int r1 = Mathf.Max(-gridWidth, -q - gridWidth);
            int r2 = Mathf.Min(gridWidth, -q + gridWidth);

            for (int r = r1; r <= r2; r++)
            {
                AddTile(new Vector3Int(q, r, -q-r));
            }
        }
    }

    
    Vector3Int GetTileDirection(int direction)
    {
        // valid range is 0 - 5
        if (direction >= 0 && direction <= 5)
        {
            return tileDirections[direction];
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    // public bool TilePlacementValidStart(HexTile tile)
    // {
    //     List<Vector3Int> neighbours = GetTileNeighbours(tile.coord);

    //     neighbours = neighbours.Intersect(completedTiles.ConvertAll(x => x.coord).Union(activeTiles.ConvertAll(x => x.coord))).ToList();

    //     foreach (int dir in tile.availableDirections)
    //     {
    //         foreach (var neighbour in neighbours)
    //         {
    //             if (GetTile(neighbour) is HexTile neighbourTile)
    //             {
    //                 if (completedTiles.Contains(neighbourTile))
    //                 {
    //                     return true;
    //                 }
    //             }
    //         }
    //     }
    //     return false;
    // }
        public bool TilePlacementValidStart(HexTile tile)
    {
        List<Vector3Int> neighbours = GetTileNeighbours(tile.coord);

        neighbours = neighbours.Intersect(completedTiles.ConvertAll(x => x.coord).Union(activeTiles.ConvertAll(x => x.coord))).ToList();

        foreach (int dir in tile.availableDirections)
        {
            foreach (var neighbour in neighbours)
            {
                if (GetTile(neighbour) is HexTile neighbourTile)
                {
                    if (completedTiles.Contains(neighbourTile))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void ButtonRotateTileLeft()
    {
        if (activeTile != null)
        {
            List<int> newRotations = new List<int>();

            for (int i = 0; i < activeTile.availableDirections.Count; i++)
            {
                int newDir = activeTile.availableDirections[i] + 1;

                if (newDir > 5)
                {
                    newDir = newDir - 6;
                }

                newRotations.Add(newDir);
            }

            activeTile.availableDirections = newRotations;
            activeTile.transform.Rotate(new Vector3(0,0,60));
        }
    }

    public bool TilePlacementValid(HexTile tile)
    {
        List<Vector3Int> neighbours = GetTileNeighbours(tile.coord);
        neighbours = neighbours.Intersect(completedTiles.ConvertAll(x => x.coord).Union(activeTiles.ConvertAll(x => x.coord))).ToList();
        //neighbours.ForEach(x => Debug.Log(x));
        bool freeExist = false;
        
        foreach (int dir in tile.availableDirections)
        {
            if (GetTile(tile.coord + GetTileDirection(dir)).tileState == TileState.Inactive)
            {
                freeExist = true;
            }
            foreach (Vector3Int neighbour in neighbours)
            {
                if (GetTile(neighbour) is HexTile neighbourTile && tile.coord + GetTileDirection(dir) == neighbourTile.coord)
                {
                    foreach (int neighDir in neighbourTile.availableDirections)
                    {
                        if (neighbourTile.coord + GetTileDirection(neighDir) == tile.coord && completedTiles.Contains(neighbourTile) && freeExist)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;

    }

    HexTile GetTile(Vector3Int cellCoordinate)
    {
        return tiles[cellCoordinate];
    }

    HexTile GetRandomTile(int row = 0, bool aboveRow = false)
    {
        List<Vector3Int> result = new List<Vector3Int>();
        if (row == 0)
        {
            return tiles.ElementAt(Random.Range(0, tiles.Count)).Value;
        }

        foreach (Vector3Int tile in tilesHash.Except(activeTiles.ConvertAll(x => x.coord)))
        {
            HashSet<int> tileList = VectorToArray(tile);

            if (tileList.Any(x => Mathf.Abs(x) == row) && tileList.All(x => Mathf.Abs(x) <= row))
            {
                result.Add(tile);
            }
        }
        if (result.Count > 0)
        {
            return GetTile(result[Random.Range(0, result.Count)]);
        }
        else
        {
            return null;
        }
    }

    HashSet<int> VectorToArray(Vector3Int coord)
    {
        HashSet<int> result = new HashSet<int>{coord.x, coord.y, coord.z};
        return result;
    }

    List<Vector3Int> GetTileNeighbours(Vector3Int coord)
    {
        List<Vector3Int> neighbours = new List<Vector3Int>();
        
        foreach (Vector3Int dir in tileDirections)
        {
            neighbours.Add(coord + dir);
        }

        neighbours = neighbours.Intersect(tiles.Keys.ToArray()).ToList();

        return neighbours;
    }

    public List<int> AddNeighbours(int min = 2, int max = 4)
    {
        return GenerateUniqueRandomNumbers(Random.Range(min, max + 1));
    }

    List<int> GenerateUniqueRandomNumbers(int maxNumbers)
    {
        List<int> uniqueNumbers = new List<int>();
        List<int> result = new List<int>();

        for(int i = 0; i <= 5; i++)
        {
            uniqueNumbers.Add(i);
        }

        for(int i = 0; i < maxNumbers; i ++)
        {
            int randomNumber = uniqueNumbers[Random.Range(0,uniqueNumbers.Count)];
            result.Add(randomNumber);
            uniqueNumbers.Remove(randomNumber);
        }

        return result;
    } 

    public Vector3 CellPosToWorldPos(Vector3Int coord)
    {

        float width = Mathf.Sqrt(3) * (tileSize + tileGap);
        float height = 1.5f * (tileSize + tileGap);
        float x = (width * coord.x * 0.5f) - (width * coord.y * 0.5f);
        float y = height * coord.z * -1;

        return new Vector3(x, y, transform.position.z);
    }

    void AddTile(Vector3Int coord)
    {
        if (coord.x + coord.y + coord.z != 0)
        {
            Debug.Log("Invalid coord");
            return;
        }
        
        GameObject obj = Instantiate(prefab, CellPosToWorldPos(coord), transform.rotation, transform);
        obj.name = string.Format("Tile_{0}_{1}_{2}", coord.x, coord.y, coord.z);
    
        HexTile tile = obj.GetComponent<HexTile>();
        tile.coord = coord;
        tile.Init();

        if (!tiles.ContainsValue(tile))
        {
            tiles.Add(coord, tile);
            tilesHash.Add(coord);
        }
    }
}
