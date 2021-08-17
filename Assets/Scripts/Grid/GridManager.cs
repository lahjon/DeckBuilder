using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;


public class GridManager : Manager
{
    public Animator animator;
    public Canvas canvas;
    public GameObject prefab; 
    public List<Sprite> inactiveTilesSprite = new List<Sprite>();
    public List<Sprite> activeTilesSprite = new List<Sprite>();
    public int gridWidth;
    public float tileSize;
    public float tileGap;
    public Dictionary<Vector3Int, HexTile> tiles = new Dictionary<Vector3Int, HexTile>();
    public HexTile activeTile;
    public HexTile currentTile;
    public List<HexTile> completedTiles = new List<HexTile>();
    public List<HexTile> specialTiles = new List<HexTile>();
    public GridState gridState;
    public BossCounter bossCounter;
    public GameObject content;
    public HexMapController hexMapController;
    public int currentTurn;
    public bool bossStarted;
    public float hexScale = 0.3765092f;
    public bool initialized;
    int furthestRowReached;
    public HashSet<HexTile> highlightedTiles = new HashSet<HexTile>();
    public Transform tileParent, roadParent;
    public int subAct;
    public int tilesUntilBoss;
    
    

    TileEncounterType GetRandomEncounterType()
    {
        List<TileEncounterType> allTypes = new List<TileEncounterType>{
            TileEncounterType.Elite,
            TileEncounterType.Special, 
            TileEncounterType.Treasure
            };

        return allTypes[Random.Range(0, allTypes.Count)];
    }

    TileBiome GetRandomTileBiome()
    {
        List<TileBiome> allTypes = new List<TileBiome>{
            TileBiome.Snow,
            TileBiome.Desert, 
            TileBiome.Forest
            };

        return allTypes[Random.Range(0, allTypes.Count)];
    }
    
    
    public static List<Vector3Int> tileDirections = new List<Vector3Int>  {
                                                                new Vector3Int(1, -1, 0), 
                                                                new Vector3Int(1, 0, -1), 
                                                                new Vector3Int(0, 1, -1), 
                                                                new Vector3Int(-1, 1, 0), 
                                                                new Vector3Int(-1, 0, 1), 
                                                                new Vector3Int(0, -1, 1)
                                                            };

    List<int> nrDirections = new List<int>() { 0, 1, 2, 3, 4, 5 };
    public int rotateCounter;
    public float rotationAmount;
    public Button buttonRotateLeft, buttonRotateRight, buttonRotateConfirm; 

    protected override void Awake()
    {
        base.Awake();
        world.gridManager = this;
        animator = GetComponent<Animator>();
        hexMapController = GetComponent<HexMapController>();
    }
    protected override void Start()
    {
        base.Start();
    } 

    public void GenerateMap()
    {
        if (!initialized)
        {
            InitializeMap();
            StartCoroutine(CreateMap());
        }
    }

    public bool CheckClearCondition()
    {
        // each time the map enters idle this checks to see if the condition from the world encounter is completed
        if (WorldSystem.instance.worldMapManager.currentWorldEncounter is WorldEncounter enc && enc.completed)
        {
            WorldStateSystem.SetInOverworld(false);
            WorldStateSystem.SetInTown(true);
            DeleteMap();
            return true;
        }
        else
            return false;
    }
  

    public void ButtonCompleteCurrentTile()
    {
        CompleteCurrentTile();
    }
    public void ButtonConfirm()
    {
        if (TilePlacementValid(activeTile))
        {
            activeTile.EndPlacement();
        }
        else
        {
            Debug.Log("FAIL");
        }
    }

    public void DeleteMap()
    {
        initialized = false;
        activeTile = null;
        currentTile = null;
        tiles.Clear();
        completedTiles.Clear();
        specialTiles.Clear();
        highlightedTiles.Clear();
        gridState = GridState.Creating;
        bossCounter.ResetCounter();
        furthestRowReached = 0;
        subAct = 0;
        for (int i = 0; i < tileParent.childCount; i++)
        {
            Destroy(tileParent.GetChild(i).gameObject);
        }
        for (int i = 0; i < roadParent.childCount; i++)
        {
            Destroy(roadParent.GetChild(i).gameObject);
        }
    }

    IEnumerator CreateMap()
    {
        bossCounter.tilesUntilBoss = tilesUntilBoss;
        bossCounter.counter = tilesUntilBoss;
        
        float timeMultiplier = .5f;
        hexMapController.disablePanning = true;
        hexMapController.disableZoom = true;
        gridState = GridState.Creating;

        float timer = 0;

        // create a 0,0,0 start tile and activate it
        HexTile firstTile = GetTile(Vector3Int.zero);
        firstTile.gameObject.SetActive(true);
        firstTile.spriteRenderer.sprite = activeTilesSprite[(int)firstTile.tileBiome];
        firstTile.LockDirections();

        // flip it up
        timer = 1 * timeMultiplier;
        firstTile.transform.DOScale(hexScale, timer).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(timer);

        yield return StartCoroutine(firstTile.AnimateVisible());

        hexMapController.Zoom(ZoomState.Outer, null, true);
        yield return new WaitForSeconds(1);

        timer = 0.3f * timeMultiplier;

        for (int i = 1; i <= 4; i++)
        {
            foreach (HexTile tile in GetTilesAtRow(i))
            {
                tile.gameObject.SetActive(true);
                tile.transform.DOScale(hexScale, timer).SetEase(Ease.InExpo);
            }
            yield return new WaitForSeconds(timer);
        }

        // create some randoms tiles spread out on third row
        for (int i = 0; i < 4; i++)
        {
            HexTile tile = GetRandomTile(3);
            tile.tileState = TileState.Inactive;
            tile.specialTile = true;
            timer = tile.BeginFlipUpNewTile() +.2f;
            yield return new WaitForSeconds(timer * timeMultiplier); 
        }

        yield return new WaitForSeconds(timer * timeMultiplier);

        firstTile.tileState = TileState.Completed;
        hexMapController.disablePanning = false;
        hexMapController.disableZoom = false;
        initialized = true;
        HighlightEntries(); 
        
        world.uiManager.UIWarningController.CreateWarning(bossCounter.tilesLeftUntilBoss, 3f);
    }

    public void ExpandMap()
    {
        hexMapController.disablePanning = true;
        gridWidth++;
        CreateRow(gridWidth);
        GetTilesAtRow(gridWidth).ForEach(x => 
            x.transform.DOScale(hexScale, 1).SetEase(Ease.InExpo).SetLoops(1, LoopType.Yoyo).OnComplete(() => hexMapController.disablePanning = false)
        );
    }

    public void StartBoss()
    {
        bossStarted = true;
        world.uiManager.UIWarningController.CreateWarning("Starting Boss fight!");
    }
    public void CompleteEmptyTile(HexTile tile)
    {
        if (tile != null)
        {
            List<int> tileList = new List<int>();
            VectorToArray(tile.coord).ToList().ForEach(x => tileList.Add(Mathf.Abs(x)));
            tile.CloseExists();
            tileList.Sort();

            if (tileList[tileList.Count - 1] > furthestRowReached)
            {
                furthestRowReached = tileList[tileList.Count - 1];
            }

            tile.tileState = TileState.Completed;
        }
    }

    public void CompleteCurrentTile()
    {
        if (currentTile != null)
        {
            bossCounter.counter--;
            List<int> tileList = new List<int>();
            VectorToArray(currentTile.coord).ToList().ForEach(x => tileList.Add(Mathf.Abs(x)));
            currentTile.CloseExists();
            tileList.Sort();

            if (tileList[tileList.Count - 1] > furthestRowReached)
            {
                furthestRowReached = tileList[tileList.Count - 1];
            }

            currentTile.tileState = TileState.Completed;
            animator.SetBool("IsPlaying", false);

            EventManager.CompleteTile();
        }
    }
    
    public void AddRandomExit(int row)
    {
        Debug.Log("stranger danger!");
        if (row == 0) return;

        HexTile tile = GetRandomTile(row);
        if (tile == null) 
            AddRandomExit(row + 1);
        else 
            tile.FlipEmptyTile();
    }
    public void HighlightEntries()
    {
        highlightedTiles.Clear();
        if (initialized)
        {
            HashSet<Vector3Int> openSlots = new HashSet<Vector3Int>();
            foreach (HexTile tile in completedTiles)
            {
                openSlots.UnionWith(CheckOpenNeighbours(tile));
            }
            if (bossStarted) 
            {
                // try getting the latest completed tile and get any open paths
                int itemMaxHeight = completedTiles.Max(x => x.turnCompleted);
                HexTile tile = completedTiles.Where(x => x.turnCompleted == itemMaxHeight).FirstOrDefault();
                HashSet<Vector3Int> prioSlot = CheckOpenNeighbours(tile);

                // set tile if OK, else get all open
                if(prioSlot.Count > 0 && GetTile(prioSlot.ToList()[0]) is HexTile t) 
                    tile = t;
                else
                {
                    List<HexTile> allOpen = new List<HexTile>();
                    foreach (Vector3Int coord in openSlots)
                    {
                        if (GetTile(coord) is HexTile t1)
                            allOpen.Add(t1);
                    }
                    // if all are special tiles, pick a special, else prioritize normal tiles
                    if (allOpen.All(x => x.specialTile == true))
                        tile = allOpen[Random.Range(0, allOpen.Count)];
                    else
                        tile = allOpen.Except(allOpen.Where(x => x.specialTile == true)).ToList()[Random.Range(0, allOpen.Count)];
                }
                
                tile.spriteRenderer.sprite = inactiveTilesSprite[inactiveTilesSprite.Count - 1];
                highlightedTiles.Add(tile);
                tile.tileState = TileState.InactiveHighlight;
                
            }
            else if (openSlots.Count > 0)
            {
                foreach (Vector3Int coord in openSlots)
                {
                    if (GetTile(coord) is HexTile tile)
                    {
                        
                        highlightedTiles.Add(tile);
                        tile.tileState = TileState.InactiveHighlight;
                    }
                }
            }
            else
            {
                AddRandomExit(furthestRowReached);
            }
        }
    }

    public void UpdateIcons()
    {
        foreach (HexTile tile in tiles.Values.ToList())
        {
            if (tile.specialTile)
            {
                tile.SetSpecialImage();
            }
            else if (tile.tileState == TileState.Inactive)
            {
                tile.spriteRenderer.sprite = inactiveTilesSprite[0];
            }

        }
    }

    public void ExitPlacement()
    {
        activeTile.tileState = TileState.Current;
        hexMapController.disablePanning = false;
        activeTile = null;
        animator.SetBool("Confirm", true);
    }

    void InitializeMap()
    {
        // create a hex shaped map och inactive tiles
        subAct = 1;
        gridWidth = 3;

        HexTile tile;
        for (int q = -gridWidth; q <= gridWidth; q++)
        {
            int r1 = Mathf.Max(-gridWidth, -q - gridWidth);
            int r2 = Mathf.Min(gridWidth, -q + gridWidth);

            for (int r = r1; r <= r2; r++)
            {
                tile = AddTile(new Vector3Int(q, r, -q-r));
                tile.transform.localScale = Vector3.zero;
                tile.tileState = TileState.Inactive;
                tile.gameObject.SetActive(false);
            }
        }
    }

    void CreateRow(int row)
    {
        List<Vector3> newRow = new List<Vector3>();
        HexTile tile;
        for (int q = -gridWidth; q <= gridWidth; q++)
        {
            int r1 = Mathf.Max(-gridWidth, -q - gridWidth);
            int r2 = Mathf.Min(gridWidth, -q + gridWidth);
            for (int r = r1; r <= r2; r++)
            {
                if (Mathf.Abs(q) == row || Mathf.Abs(r) == row || Mathf.Abs(-q-r) == row)
                {
                    tile = AddTile(new Vector3Int(q, r, -q-r));
                    tile.transform.localScale = Vector3.zero;
                }
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

    public void ButtonRotate(bool clockwise)
    {
        if (activeTile != null) activeTile.RotateTile(clockwise);
        
    }

    public int InvertDirection(int direction) => (direction - 3 + 6) % 6;

    public bool CheckFreeSlot(HexTile tile)
    {
        List<Vector3Int> neighbours = GetNeighboursCoords(tile.coord);
        neighbours = neighbours.Intersect(completedTiles.ConvertAll(x => x.coord)).ToList();

        foreach (Vector3Int neighbour in neighbours)
        {
            if (GetTile(neighbour) is HexTile neighbourTile)
            {
                foreach (int dir in neighbourTile.availableDirections)
                {
                    if((neighbour + GetTileDirection(dir)) == tile.coord)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    HashSet<Vector3Int> CheckOpenNeighbours(HexTile tile)
    {
        // get all the neighbours of a tile and discard all inactive tiles
        List<Vector3Int> neighboursList = GetNeighboursCoords(tile.coord);
        neighboursList = neighboursList.Except(completedTiles.ConvertAll(x => x.coord)).ToList();
        HashSet<Vector3Int> neighbours = new HashSet<Vector3Int>(neighboursList);
        HashSet<Vector3Int> result = new HashSet<Vector3Int>();

        foreach (int dir in tile.availableDirections)
        {
            if(neighbours.Contains(tile.coord + GetTileDirection(dir)))
            {
                result.Add(tile.coord + GetTileDirection(dir));
            }
        }

        return result;
    }

    // used to see if the tile is connected to an exit
    public bool TileConnectedToExit(HexTile tile)
    {
        // look at all exists
        foreach (Vector3Int dir in tileDirections)
        {
            // make sure its a connected neighbour
            if (GetTile(tile.coord + dir) is HexTile neighbourTile && neighbourTile.tileState == TileState.Completed)
            {
                // look at all neighbour directions
                foreach (int neighDir in neighbourTile.availableDirections)
                {
                    // check if the neighbor tile has an avaiable direction to the initial tile
                    if (tile == GetTile(GetTileDirection(neighDir) + neighbourTile.coord))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // used for placing a tile before rotation 
    public bool TilePlacementValidStart(HexTile tile)
    {
        // get all the neighbours of a tile and discard all inactive tiles
        List<Vector3Int> neighbours = GetNeighboursCoords(tile.coord);
        neighbours = neighbours.Intersect(completedTiles.ConvertAll(x => x.coord)).ToList();

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

    public (int, Encounter) GetEntry(HexTile tile)
    {
        List<HexTile> neighbours = GetNeighbours(tile);
        neighbours = neighbours.Intersect(completedTiles).ToList();

       // neighbours.ForEach(x => Debug.Log("NEIGHBOYURS: " + x));

        HexTile entryTile = null;
        Encounter entryHex = null;
        int turn = -1;
        int dir;

        foreach (HexTile aTile in neighbours)
        {
            if (aTile.turnCompleted > turn)
            {
                //Debug.Log("TURN OK");
                foreach (Encounter hex in aTile.encountersExits)
                {
                   // Debug.Log("tile.coord: " + tile.coord);
                    //Debug.Log("aTile.coord: " + aTile.coord);
                    dir = tileDirections.IndexOf(tile.coord - aTile.coord);
                  //  Debug.Log("DIR: " + dir);
                    Vector3Int coord = HexTile.positionsExit[dir];
                   // Debug.Log("COORD: " + coord + " = " + "HEXCORD: " + hex.coordinates);
                    
                    if (hex.ExitDirection() == dir && hex.status == EncounterHexStatus.Visited)
                    {
                       // Debug.Log("YES");
                        turn = aTile.turnCompleted;
                        entryHex = hex;
                        entryTile = aTile;
                    }
                }
            }
        }
        if (entryTile == null)
        {
           // Debug.Log("NO ENTRY!");
            return (0, null);
        }

        //Debug.Log("EntryDir: " + tileDirections.IndexOf(entryTile.coord - tile.coord));
        //Debug.Log(tileDirections.IndexOf(entryTile.coord - tile.coord));
        //Debug.Log(entryHex);

        return (tileDirections.IndexOf(entryTile.coord - tile.coord), entryHex);
    }


    public bool TilePlacementValid(HexTile tile)
    {
        // get all the neighbours of a tile and discard all inactive tiles
        List<Vector3Int> neighbours = GetNeighboursCoords(tile.coord);
        neighbours = neighbours.Intersect(completedTiles.ConvertAll(x => x.coord)).ToList();

        // bool freeExist = false;

        // // look at all exists
        // foreach (int dir in tile.availableDirections)
        // {
        //     // make sure that one exit connects to a inactive tile
        //     //Debug.Log(tile.coord);
        //     //Debug.Log(GetTileDirection(dir));
        //     if (GetTile(tile.coord + GetTileDirection(dir)) is HexTile validTile && validTile.tileState == TileState.Inactive)
        //     {
        //         Debug.Log("One Exit");
        //         freeExist = true;
        //         break;
        //     }
        // }
        
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
                        if (neighbourTile.coord + GetTileDirection(neighDir) == tile.coord && completedTiles.Contains(neighbourTile))
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
        if (tiles.ContainsKey(cellCoordinate))
        {
            return tiles[cellCoordinate];
        }
        else   
            return null;
    }

    HexTile GetRandomTile(int row = 0)
    {
        List<Vector3Int> result = new List<Vector3Int>();
        if (row == 0)
        {
            return GetTile(Vector3Int.zero);
        }

        foreach (Vector3Int tile in tiles.Keys.Except(specialTiles.ConvertAll(x => x.coord)).Except(completedTiles.ConvertAll(x => x.coord)))
        {
            HashSet<int> tileList = VectorToArray(tile);

            if (tileList.Any(x => Mathf.Abs(x) == row) && tileList.All(x => Mathf.Abs(x) <= row))
            {
                result.Add(tile);
            }
        }
        if (result.Any())
        {
            return GetTile(result[Random.Range(0, result.Count)]);
        }
        else
        {
            return null;
        }
    }

    HexTile GetRandomCompletedTile(int row)
    {
        if (completedTiles.Count == 0)
            return null;

        List<Vector3Int> result = new List<Vector3Int>();

        foreach (Vector3Int tile in completedTiles.Select(x => x.coord))
        {
            HashSet<int> tileList = VectorToArray(tile);

            if (tileList.Any(x => Mathf.Abs(x) == row) && tileList.All(x => Mathf.Abs(x) <= row))
            {
                result.Add(tile);
            }
        }

        return GetTile(result[Random.Range(0, result.Count)]);
    }

    List<HexTile> GetTilesAtRow(int row)
    {
        List<HexTile> result = new List<HexTile>();
        if (row == 0)
        {
            result.Add(GetTile(Vector3Int.zero));
        }
        else
        {
            foreach (Vector3Int tile in tiles.Keys.Except(specialTiles.ConvertAll(x => x.coord)))
            {
                HashSet<int> tileList = VectorToArray(tile);

                if (tileList.Any(x => Mathf.Abs(x) == row) && tileList.All(x => Mathf.Abs(x) <= row))
                {
                    result.Add(GetTile(tile));
                }
            }
        }

        return result;
    }

    HashSet<int> VectorToArray(Vector3Int coord)
    {
        HashSet<int> result = new HashSet<int>{coord.x, coord.y, coord.z};
        return result;
    }

    public List<int> GetNewExits(HexTile tile)
    {
        List<Vector3Int> neighbours = GetNeighboursCoords(tile.coord);
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
                        result.Add(InvertDirection(dir));
                    }
                }
            }
        }

        return result;
    }

    List<Vector3Int> GetNeighboursCoords(Vector3Int coord)
    {
        List<Vector3Int> neighbours = new List<Vector3Int>();
        
        foreach (Vector3Int dir in tileDirections)
        {
            neighbours.Add(coord + dir);
        }

        neighbours = neighbours.Intersect(tiles.Keys.ToArray()).ToList();

        return neighbours;
    }

    public List<HexTile> GetNeighbours(HexTile tile)
    {
        List<HexTile> neighbours = new List<HexTile>();
        
        foreach (Vector3Int dir in tileDirections)
        {
            if (GetTile(tile.coord + dir) is HexTile aTile)
            {
                neighbours.Add(aTile);
            }
        }
        return neighbours;
    }

    public List<int> AddNeighbours(int min = 2, int max = 4)
    {
        return GenerateUniqueRandomNumbers(Random.Range(min, max + 1));
    }

    List<int> GenerateUniqueRandomNumbers(int maxNumbers)
    {
        List<int> uniqueNumbers = new List<int>(nrDirections);
        List<int> result = new List<int>();

        for(int i = 0; i < maxNumbers; i ++)
        {
            int randomNumber = uniqueNumbers[Random.Range(0, uniqueNumbers.Count)];
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

    HexTile AddTile(Vector3Int coord)
    {
        if (coord.x + coord.y + coord.z != 0)
        {
            Debug.LogWarning("Invalid coord");
            return null;
        }
        
        GameObject obj = Instantiate(prefab, CellPosToWorldPos(coord), transform.rotation, tileParent);
        obj.name = string.Format("Tile_{0}_{1}_{2}", coord.x, coord.y, coord.z);
    
        HexTile tile = obj.GetComponent<HexTile>();
        tile.coord = coord;
        tile.tileEncounterType = GetRandomEncounterType();
        tile.tileBiome = GetRandomTileBiome();
        tile.Init();

        if (!tiles.ContainsValue(tile))
        {
            tiles.Add(coord, tile);
        }

        tile.Init();
        if (coord != Vector3.zero)
            world.encounterManager.GenerateHexEncounters(tile, new List<Vector3Int>() { Vector3Int.zero});
        else
            world.encounterManager.GenerateInitialHexEncounters(tile);
    
        tile.ContentVisible(false);
        
        return tile;
    }
}
