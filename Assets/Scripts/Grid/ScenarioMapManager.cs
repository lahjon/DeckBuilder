using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class ScenarioMapManager : Manager
{
    public Animator animator;
    public Canvas canvas;
    public GameObject prefab; 
    public List<Sprite> inactiveTilesSprite = new List<Sprite>();
    public List<Sprite> activeTilesSprite = new List<Sprite>();
    public int gridWidth;
    public float tileSize;
    public float tileGap;
    public Material undiscoveredMaterial;
    public Material discoveredMaterial;
    public Dictionary<Vector3Int, HexTile> tiles = new Dictionary<Vector3Int, HexTile>();
    public HexTile activeTile;
    public HexTile currentTile;
    public List<HexTile> completedTiles = new List<HexTile>();
    public List<HexTile> specialTiles = new List<HexTile>();
    public GridState gridState;
    public GameObject content;
    public HexMapController hexMapController;
    public int currentTurn;
    public bool bossStarted;
    //public float hexScale = 0.3765092f;
    public float hexScale = 0.392f;
    public bool initialized;
    int furthestRowReached;
    public HashSet<HexTile> highlightedTiles = new HashSet<HexTile>();
    public Transform tileParent, roadParent;
    public int subAct;
    ConditionType conditionType;
    public TMP_Text conditionText;

    public List<HexTile> choosableTiles = new List<HexTile>();

    public Encounter finishedEncounterToReport;

    public ObjectiveDisplayer objectiveDisplayer;
    
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
    
    
    public static List<GridDirection> tileDirections = new List<GridDirection>  {
                                                                new GridDirection(GridDirection.DirectionName.East), 
                                                                new GridDirection(GridDirection.DirectionName.NorthEast), 
                                                                new GridDirection(GridDirection.DirectionName.NorthWest), 
                                                                new GridDirection(GridDirection.DirectionName.West), 
                                                                new GridDirection(GridDirection.DirectionName.SouthWest), 
                                                                new GridDirection(GridDirection.DirectionName.SouthEast)
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
        Debug.Log("Generating Map");
        if (!initialized)
        {
            InitializeMap();
            StartCoroutine(CreateMap());
        }
    }

    public void CheckClearCondition()
    {
        // each time the map enters idle this checks to see if the condition from the world encounter is completed
        if (WorldSystem.instance.worldMapManager.currentWorldEncounter is Scenario enc && enc.completed)
        {
            DeleteMap();
            WorldStateSystem.SetInTown(true);
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

    public void SetRandomTileImage(HexTile tile)
    {
        tile.spriteRenderer.sprite = activeTilesSprite[Random.Range(0, activeTilesSprite.Count())];
        tile.undiscoveredSpriteRenderer.sprite = inactiveTilesSprite[Random.Range(0, inactiveTilesSprite.Count())];
    }

    IEnumerator CreateMap()
    {
        float timeMultiplier = .5f;
        hexMapController.disablePanning = true;
        hexMapController.disableZoom = true;
        hexMapController.enableInput = false;
        gridState = GridState.Creating;

        // create a 0,0,0 start tile and activate it
        HexTile firstTile = GetTile(Vector3Int.zero);
        firstTile.transform.localScale = Vector3.one * hexScale;
        firstTile.gameObject.SetActive(true);
        SetRandomTileImage(firstTile);
        // flip it up
        float timer = 0.3f * timeMultiplier;

        hexMapController.Zoom(ZoomState.Outer, null, true);
        yield return new WaitForSeconds(1);

        for (int i = 1; i <= 4; i++)
        {
            foreach (HexTile tile in GetTilesAtRow(i))
            {
                tile.gameObject.SetActive(true);
                SetRandomTileImage(tile);
                tile.transform.DOScale(hexScale, timer).SetEase(Ease.InExpo);
            }
            yield return new WaitForSeconds(timer);
        }


        yield return new WaitForSeconds(timer * timeMultiplier);

        firstTile.RevealTile();
        yield return new WaitForSeconds(timer);

        yield return StartCoroutine(firstTile.AnimateVisible());


        foreach (GridDirection dir in firstTile.availableDirections)
        {
            HexTile tile = GetTile(dir);
            choosableTiles.Add(tile);
            tile.entryDirection = dir;
        }

        firstTile.tileState = TileState.Completed;
        hexMapController.disablePanning = false;
        hexMapController.disableZoom = false;
        initialized = true;

        HighlightChoosable();

        WorldSystem.instance.worldMapManager.currentWorldEncounter?.SetupInitialSegments();
        hexMapController.enableInput = true;
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

    public void CompleteCurrentTile()
    {
        if (currentTile != null)
        {
            List<int> tileList = new List<int>();
            VectorToArray(currentTile.coord).ToList().ForEach(x => tileList.Add(Mathf.Abs(x)));
            tileList.Sort();

            if (tileList[tileList.Count - 1] > furthestRowReached)
            {
                furthestRowReached = tileList[tileList.Count - 1];
            }

            currentTile.tileState = TileState.Completed;

            Encounter currentEnc = WorldSystem.instance.encounterManager.currentEncounter;
            GridDirection dir = currentTile.exitEncToDirection[currentEnc];

            if (currentEnc != null && GetTile(currentTile.coord + dir) is HexTile targetTile)
            {
                choosableTiles.Add(targetTile);
                targetTile.entryDirection = dir;
            }

            animator.SetBool("IsPlaying", false);

            CheckClearCondition();
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
            choosableTiles.Add(tile);
    }
    public void HighlightChoosable()
    {
        highlightedTiles.Clear();
        if (initialized)
        {
            if(choosableTiles.Count > 0)
            {
                choosableTiles.ForEach(x =>
                {
                    highlightedTiles.Add(x);
                    x.tileState = TileState.InactiveHighlight;
                });
            }
            else
            {
                AddRandomExit(furthestRowReached);
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
        //gridWidth = 3;

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

        tiles.Values.ToList().ForEach(x => x.AddNeighboors());
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

    public void SetButtonsInteractable(bool val)
    {
        buttonRotateConfirm.interactable = val;
        buttonRotateLeft.interactable = val;
        buttonRotateRight.interactable = val;
    }

    public void ButtonRotate(bool clockwise)
    {
        if (activeTile != null) activeTile.RotateTile(clockwise);
    }

    public bool TilePlacementValid(HexTile tile) => tile.availableDirections.Contains(tile.entryDirection.GetOpposing());

    public HexTile GetTile(Vector3Int cellCoordinate) => tiles.ContainsKey(cellCoordinate) ? tiles[cellCoordinate] : null;
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

    public Vector3 CellPosToWorldPos(Vector3Int coord)
    {
        // get world position of the coord
        float width = Mathf.Sqrt(3) * (tileSize + tileGap);
        float height = 1.5f * (tileSize + tileGap);
        float x = (width * coord.x * 0.5f) - (width * coord.y * 0.5f);
        float y = height * coord.z * -1;

        return new Vector3(x, y, transform.position.z);
    }

    public HexTile AddTile(Vector3Int coord)
    {
        System.Array tileTypes = System.Enum.GetValues(typeof(TileType));

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
        tile.type = (TileType)tileTypes.GetValue(Random.Range(0, tileTypes.Length));

        tiles.Add(coord, tile);

        if (coord == Vector3.zero)
            world.encounterManager.GenerateFirstHexEncounters(tile);
    
        tile.ContentVisible(false);
        
        return tile;
    }

    public void ReportEncounter()
    {
        if (finishedEncounterToReport is null) return;
        EventManager.EncounterCompleted(finishedEncounterToReport);

    }
}