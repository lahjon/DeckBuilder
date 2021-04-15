using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using TMPro;


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
    public List<HexTile> activeTiles = new List<HexTile>();
    public Vector3 hoverTilePosition = Vector3.zero;
    public HexTile hoverTile;
    public HexTile oldHoverTile;
    public HexTile activeTile;
    public List<HexTile> completedTiles = new List<HexTile>();
    public GridState gridState;
    public TMP_Text bossCounterText;
    int _bossCounter;
    public int bossCounter
    {
        get
        {
            return _bossCounter;
        }

        set 
        {
            _bossCounter = value;
            bossCounterText.text = _bossCounter.ToString();
            LeanTween.scale(bossCounterText.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.2f).setEaseInBounce().setLoopPingPong(2);
        }
    }
    
    
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

        // create a 0,0,0 start tile
        GetTile(Vector3Int.zero).Activate(TileState.Completed);

        // create some randoms tiles spread out on third row
        for (int i = 0; i < 4; i++)
        {
            GetRandomTile(3).Activate();
        }

        // add some tiles to inventory to test placement
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

    public void IsComplete()
    {
        animator.SetBool("IsComplete", true);
    }

    public void ButtonCompletePlacement()
    {
        if (TilePlacementValid(activeTile))
        {
            activeTile.ConfirmTilePlacement();
            IsComplete();
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
        // create a hex shaped map och inactive tiles
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

    Sprite GetSprite(TileState tileState)
    {
        switch (tileState)
        {
            case TileState.Inactive:
                return sprites[1];

            case TileState.Active:
                return sprites[0];
            
            default:
                return sprites[0];
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
    public void ButtonRotate(bool clockwise)
    {
        int sign = clockwise ? 1 : -1;
        if (activeTile != null)
        {
            for (int i = 0; i < activeTile.availableDirections.Count; i++)
                activeTile.availableDirections[i] = (activeTile.availableDirections[i] + sign + 6) % 6;
            
            activeTile.transform.Rotate(new Vector3(0,0,sign*60));
        }
    }

    public bool CheckFreeSlot(HexTile tile)
    {
        List<Vector3Int> neighbours = GetTileNeighbours(tile.coord);
        neighbours = neighbours.Intersect(completedTiles.ConvertAll(x => x.coord)).ToList();

        foreach (Vector3Int neighbour in neighbours)
        {
            if (GetTile(neighbour) is HexTile neighbourTile)
            {
                foreach (int dir in neighbourTile.availableDirections)
                {
                    if((neighbour + GetTileDirection(dir)) == tile.coord)
                        return true;
                }
            }
        }

        return false;
    }

    //used for placing a tile before rotation 
    public bool TilePlacementValidStart(HexTile tile)
    {
        // get all the neighbours of a tile and discard all inactive tiles
        List<Vector3Int> neighbours = GetTileNeighbours(tile.coord);
        neighbours = neighbours.Intersect(completedTiles.ConvertAll(x => x.coord).Union(activeTiles.ConvertAll(x => x.coord))).ToList();

        // look at all exists
        foreach (int dir in tile.availableDirections)
        {
            // look at all neighbours of the tile
            foreach (Vector3Int neighbour in neighbours)
            {
                // look if any neighbour tile is completed
                if (GetTile(neighbour) is HexTile neighbourTile && completedTiles.Contains(neighbourTile))
                {
                        return true;
                }
            }
        }
        return false;
    }



    public bool TilePlacementValid(HexTile tile)
    {
        // get all the neighbours of a tile and discard all inactive tiles
        List<Vector3Int> neighbours = GetTileNeighbours(tile.coord);
        neighbours = neighbours.Intersect(completedTiles.ConvertAll(x => x.coord).Union(activeTiles.ConvertAll(x => x.coord))).ToList();

        bool freeExist = false;

        // look at all exists
        foreach (int dir in tile.availableDirections)
        {
            // make sure that one exit connects to a inactive tile
            if (GetTile(tile.coord + GetTileDirection(dir)).tileState == TileState.Inactive)
            {
                freeExist = true;
                break;
            }
        }
        
        // look at all exists
        foreach (int dir in tile.availableDirections)
        {
            // look at all neighbours of the tile
            foreach (Vector3Int neighbour in neighbours)
            {
                // get the neighbours of the tile that are connected
                if (GetTile(neighbour) is HexTile neighbourTile && tile.coord + GetTileDirection(dir) == neighbourTile.coord)
                {
                    // look at all neighbours
                    foreach (int neighDir in neighbourTile.availableDirections)
                    {
                        // neighbour connects to the piece, the piece is connected to at least one completed tile and there is one exit to an inactive tile
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

        foreach (Vector3Int tile in tiles.Keys.Except(activeTiles.ConvertAll(x => x.coord)))
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

    public List<int> GetNewExits(HexTile tile)
    {
        List<Vector3Int> neighbours = GetTileNeighbours(tile.coord);
        neighbours = neighbours.Intersect(completedTiles.ConvertAll(x => x.coord)).ToList();
        List<int> result = new List<int>();

        foreach (Vector3Int neighbour in neighbours)
        {
            if (GetTile(neighbour) is HexTile neighbourTile)
            {
                foreach (int dir in neighbourTile.availableDirections)
                {
                    if((neighbour + GetTileDirection(dir)) == tile.coord)
                    {
                        result.Add((dir - 3 + 6) % 6);
                    }
                }
            }
        }

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
        // get world position of the coord
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
        }
    }
}
