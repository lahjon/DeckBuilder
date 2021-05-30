using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
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
    public TMP_Text bossCounterText;
    public GameObject content;
    public HexMapController hexMapController;
    public int currentTurn;
    int _bossCounter;
    int tilesUntilBoss;
    float hexScale = 0.3765092f;
    public bool initialized;
    int furthestRowReached;
    public HashSet<HexTile> highlightedTiles = new HashSet<HexTile>();
    public Transform tileParent, roadParent;
    public int subAct;
    public int bossCounter
    {
        get
        {
            return _bossCounter;
        }

        set 
        {
            _bossCounter = value;
            bossCounterText.text = (tilesUntilBoss - _bossCounter).ToString();
            float a = (float)_bossCounter;
            float b = (float)tilesUntilBoss;
            float textScale = 1.0f + (a / b) * 0.5f;
            LeanTween.scale(bossCounterText.gameObject, new Vector3(textScale, textScale, textScale), 0.2f).setEaseInBounce().setLoopPingPong(2);
            if (_bossCounter >= tilesUntilBoss)
            {
                StartBoss();
            }
        }
    }

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
    int rotateCounter;
    float rotationAmount;
    [SerializeField] Button buttonRotateLeft, buttonRotateRight; 

    protected override void Awake()
    {
        base.Awake();
        world.gridManager = this;
        animator = GetComponent<Animator>();
        hexMapController = GetComponent<HexMapController>();
        tilesUntilBoss = 10;
    }
    protected override void Start()
    {
        base.Start();
    } 

    public void ButtonCreateMap()
    {
        
        if (!initialized)
        {
            InitializeMap();
            //DEBUG F�R ATT KUNNA TESTA: 
            WorldStateSystem.SetInTown(false);
            WorldStateSystem.SetInOverworld(true);
            StartCoroutine(CreateMap());
        }
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
        _bossCounter = tilesUntilBoss;
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
        float timeMultiplier = .5f;
        hexMapController.disableInput = true;
        gridState = GridState.Creating;

        float timer = 0;

        // create a 0,0,0 start tile and activate it
        HexTile firstTile = GetTile(Vector3Int.zero);
        firstTile.gameObject.SetActive(true);
        //world.encounterManager.GenerateHexEncounters(firstTile);
        firstTile.Activate();
        firstTile.spriteRenderer.sprite = activeTilesSprite[(int)firstTile.tileBiome];
        firstTile.LockDirections();

        // flip it up
        timer = 1 * timeMultiplier;
        firstTile.transform.DOScale(hexScale, timer).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(timer);

        WorldSystem.instance.encounterManager.GenerateInitialHex(firstTile);
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
            tile.tileState = TileState.Special;
            timer = tile.BeginFlipUpNewTile() +.2f;
            yield return new WaitForSeconds(timer * timeMultiplier); 
        }

        yield return new WaitForSeconds(timer * timeMultiplier);

        firstTile.tileState = TileState.Current;
        hexMapController.disableInput = false;
        animator.SetBool("IsPlaying", true);
        initialized = true;
    }

    public void ExpandMap()
    {
        hexMapController.disableInput = true;
        gridWidth++;
        CreateRow(gridWidth);
        GetTilesAtRow(gridWidth).ForEach(x => 
            x.transform.DOScale(hexScale, 1).SetEase(Ease.InExpo).SetLoops(1, LoopType.Yoyo).OnComplete(() => hexMapController.disableInput = false)
        );
    }

    void StartBoss()
    {
        world.uiManager.UIWarningController.CreateWarning("Starting Boss fight!");
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

    

    public void CompleteCurrentTile()
    {
        if (currentTile != null)
        {
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
        }
    }

    public void CreateBossTile()
    {
        HexTile tile = highlightedTiles.ToArray().Except(specialTiles).ToArray()[Random.Range(0, highlightedTiles.Count)];
        Debug.Log(tile);
        tile.BeginFlipUpNewTile();
    }

    public void AddRandomExit()
    {
        HexTile tile = GetRandomCompletedTile(furthestRowReached);
        // ADD TO TAKE A ECNOUTER AND COMPLETE IT INSTEAD

        HighlightEntries();
        
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
            foreach (Vector3Int coord in openSlots)
            {
                if (GetTile(coord) is HexTile tile)
                {
                    highlightedTiles.Add(tile);
                    tile.tileState = TileState.InactiveHighlight;
                }
            }
        }
    }

    public void ExitPlacement()
    {
        activeTile.tileState = TileState.Current;
        hexMapController.disableInput = false;
        activeTile = null;
        animator.SetBool("Confirm", true);
    }

    void InitializeMap()
    {
        // create a hex shaped map och inactive tiles
        subAct = 1;
        gridWidth = 3;
        bossCounter = 0;

        HexTile tile;
        for (int q = -gridWidth; q <= gridWidth; q++)
        {
            int r1 = Mathf.Max(-gridWidth, -q - gridWidth);
            int r2 = Mathf.Min(gridWidth, -q + gridWidth);

            for (int r = r1; r <= r2; r++)
            {
                tile = AddTile(new Vector3Int(q, r, -q-r));
                //WorldSystem.instance.encounterManager.GenerateHexEncounters(tile);
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
        if (activeTile is null) return;

        if (rotateCounter > 6)
        {
            Debug.Log("Shouldnt Happen");
            rotateCounter = 0;
            return;
        }

        if (activeTile.encounterEntry.Item1 != null)
            activeTile.encounterEntry.Item1.CancelAnimation();

        int sign = clockwise ? 1 : -1;
        for (int i = 0; i < activeTile.availableDirections.Count; i++)
            activeTile.availableDirections[i] = (activeTile.availableDirections[i] + sign + 6) % 6;

        for (int i = 0; i < activeTile.encountersExits.Count; i++)
            activeTile.encountersExits[activeTile.encountersExits.ElementAt(i).Key] = (activeTile.encountersExits[activeTile.encountersExits.ElementAt(i).Key] + sign + 6) % 6;

        rotationAmount += sign*60;

        if (!TilePlacementValid(activeTile))
        {
            ButtonRotate(clockwise);
            rotateCounter++;
        }
        else
        {
            buttonRotateLeft.enabled = false;
            buttonRotateRight.enabled = false;
            activeTile.OffsetRotation();
            
            activeTile.transform.DORotate(new Vector3(0, 0, activeTile.transform.localRotation.eulerAngles.z + rotationAmount), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.InExpo).OnComplete(() => {
                buttonRotateLeft.enabled = true;
                buttonRotateRight.enabled = true;
                rotationAmount = 0;
                rotateCounter = 0;
                activeTile.encountersExits.Keys.ToList().ForEach(x => x.UpdateEntry());
                activeTile.MatchRotation();
            });
        }

            
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

    public (int, EncounterHex) GetEntry(HexTile tile)
    {
        List<HexTile> neighbours = GetNeighbours(tile);
        neighbours = neighbours.Intersect(completedTiles).ToList();

        HexTile entryTile = null;
        EncounterHex entryHex = null;
        int turn = -1;
        int dir;

        foreach (HexTile aTile in neighbours)
        {
            if (aTile.turnCompleted > turn)
            {
                foreach (EncounterHex hex in aTile.encountersExits.Keys)
                {
                    dir = tileDirections.IndexOf(tile.coord - aTile.coord);
                    Vector3Int coord = HexTile.positionsExit[dir];
                    if (hex.coordinates == coord)
                    {
                        turn = aTile.turnCompleted;
                        entryHex = hex;
                        entryTile = aTile;
                    }
                }
            }
        }

        Debug.Log("EntryDir: " + tileDirections.IndexOf(entryTile.coord - tile.coord));

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

    // public void CloseExists(HexTile tile)
    // {
    //     List<int> openExists = new List<int>();

    //     // get all the neighbours of a tile and discard all inactive tiles
    //     List<Vector3Int> neighbours = GetNeighboursCoords(tile.coord);
    //     neighbours = neighbours.Intersect(completedTiles.ConvertAll(x => x.coord)).ToList();

    //     // look at all exists
    //     foreach (int dir in tile.availableDirections)
    //     {
    //         openExists.Clear();
    //         // look at all neighbours of the tile
    //         foreach (Vector3Int neighbour in neighbours)
    //         {
    //             // get the neighbours of the tile that are connected
    //             if (GetTile(neighbour) is HexTile neighbourTile && tile.coord + GetTileDirection(dir) == neighbourTile.coord)
    //             {
    //                 foreach (int neighDir in neighbourTile.availableDirections)
    //                 {
    //                     if (GetTile(neighbourTile.coord + GetTileDirection(neighDir)) is HexTile aTile)
    //                     {
    //                         if (aTile.tileState == TileState.Completed || aTile.tileState == TileState.Active)
    //                         {
    //                             openExists.Add(neighDir);
    //                         }
    //                     }
    //                 }
    //                 openExists.Add(InvertDirection(dir));
    //                 neighbourTile.CloseExits(openExists);
    //             }
    //         }
    //     }
    // }

    


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

        foreach (Vector3Int tile in tiles.Keys.Except(specialTiles.ConvertAll(x => x.coord)))
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
        return tile;
    }
}
