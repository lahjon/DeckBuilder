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
    public GameObject HexTilePrefab; 
    public List<Sprite> inactiveTilesSprite = new List<Sprite>();
    public List<Sprite> activeTilesSprite = new List<Sprite>();
    public int gridWidth;
    public float tileSize, tileGap;
    public Material undiscoveredMaterial, discoveredMaterial;
    public Dictionary<Vector3Int, HexTile> tiles = new Dictionary<Vector3Int, HexTile>();
    public HexTile currentTile;
    public List<HexTile> completedTiles = new List<HexTile>();
    public GridState gridState;
    public GameObject content;
    public HexMapController hexMapController;
    public int currentTurn;
    public float hexScale = 0.392f;
    public bool initialized;
    int furthestRowReached;
    public Transform tileParent, roadParent;
    public TMP_Text conditionText;

    public HashSet<HexTile> choosableTiles = new HashSet<HexTile>();

    [HideInInspector]
    public Encounter finishedEncounterToReport;
    public ObjectiveDisplayer objectiveDisplayer;

    public List<EncounterDataCombat> allCombatEncounters = new List<EncounterDataCombat>();
    public List<EncounterDataCombat> availableCombatEncounters = new List<EncounterDataCombat>();
    public List<EncounterDataCombat> completedCombatEncounters = new List<EncounterDataCombat>();
    public List<EncounterDataRandomEvent> allChoiceEncounters = new List<EncounterDataRandomEvent>();
    public List<EncounterDataRandomEvent> availableChoiceEncounters = new List<EncounterDataRandomEvent>();
    public List<EncounterDataRandomEvent> completedChoiceEncounters = new List<EncounterDataRandomEvent>();
    public ScenarioData scenarioData;
    
    
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
    
    public EncounterDataCombat GetRndEncounterCombat(ScenarioEncounterType type)
    {
        if (!availableCombatEncounters.Any(e => (int)e.type == (int)type)) ResetEncountersCombatToDraw((CombatEncounterType)type);
        int id = Random.Range(0, availableCombatEncounters.Count);
        EncounterDataCombat data = availableCombatEncounters[id];
        availableCombatEncounters.RemoveAt(id);
        completedCombatEncounters.Add(data);
        return data;
    }

    public EncounterDataRandomEvent GetRndEncounterChoice()
    {
        if (!availableChoiceEncounters.Any()) ResetEncountersEventToDraw();
        int id = Random.Range(0, availableChoiceEncounters.Count);
        EncounterDataRandomEvent data = availableChoiceEncounters[id];
        availableChoiceEncounters.RemoveAt(id);
        completedChoiceEncounters.Add(data);
        return data;
    }

    public void ResetEncounters()
    {
        completedCombatEncounters.Clear();
        completedChoiceEncounters.Clear();
        EncounterFilter ef = new EncounterFilter(scenarioData);
        allChoiceEncounters = DatabaseSystem.instance.GetChoiceEncounters(ef).ToList();
        allCombatEncounters = DatabaseSystem.instance.GetCombatEncounters(ef).ToList();
        availableCombatEncounters = allCombatEncounters.ToList();
        availableChoiceEncounters = allChoiceEncounters.ToList();
    }

    public void ResetEncountersCombatToDraw(CombatEncounterType? type)
    {
        if (type != null)
        {
            availableCombatEncounters = allCombatEncounters.Where(e => e.type != type).ToList(); //paranoia;
            availableCombatEncounters.AddRange(allCombatEncounters.Where(e => e.type == type));
        }
    }

    public void ResetEncountersEventToDraw()
    {
        availableChoiceEncounters.AddRange(allChoiceEncounters);
    }

    public void GenerateMap()
    {
        if (!initialized) StartCoroutine(CreateMap());
    }

    public void CheckClearCondition()
    {
        // each time the map enters idle this checks to see if the condition from the world encounter is completed
        if (WorldSystem.instance.worldMapManager.currentWorldScenario is Scenario enc && enc.completed)
            DeleteMap();
    }

    public void ButtonCompleteCurrentTile()
    {
        CompleteCurrentTile();
    }
    public void ButtonConfirm() => currentTile.EndPlacement();
  
    public void DeleteMap()
    {
        initialized = false;
        currentTile = null;
        tiles.Clear();
        completedTiles.Clear();
        gridState = GridState.Creating;
        furthestRowReached = 0;
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

        for (int i = 0; i <= gridWidth; i++)
            CreateRow(i);

        tiles.Values.ToList().ForEach(x => x.AddNeighboors());
        hexMapController.enableInput = false;
        gridState = GridState.Creating;

        // create a 0,0,0 start tile and activate it
        HexTile firstTile = GetTile(Vector3Int.zero);
        firstTile.transform.localScale = Vector3.one * hexScale;
        firstTile.gameObject.SetActive(true);
        // flip it up
        float timer = 0.3f * timeMultiplier;

        hexMapController.Zoom(ZoomState.Outer, null);
        yield return new WaitForSeconds(1);

        for (int i = 0; i <= gridWidth; i++)
        {
            foreach (HexTile tile in GetTilesAtRow(i))
                tile.transform.DOScale(hexScale, timer).SetEase(Ease.InExpo);

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
            tile.directionEntry = dir.GetOpposing();
        }

        initialized = true;

        HighlightChoosable();

        WorldSystem.instance.worldMapManager.currentWorldScenario?.SetupInitialSegments();
        hexMapController.enableInput = true;
    }

    public void ExpandMap()
    {
        gridWidth++;
        CreateRow(gridWidth);
        GetTilesAtRow(gridWidth).ForEach(x => 
            x.transform.DOScale(hexScale, 1).SetEase(Ease.InExpo).SetLoops(1, LoopType.Yoyo).OnComplete(() => hexMapController.enableInput = false)
        );
    }

    public void CompleteCurrentTile()
    {
        if (currentTile != null)
        {
            List<int> tileCoords = new List<int>() { currentTile.coord.x, currentTile.coord.y, currentTile.coord.z };
            furthestRowReached = Mathf.Max(furthestRowReached, tileCoords.Select(x => Mathf.Abs(x)).Max());

            currentTile.tileState = TileState.Completed;

            Encounter currentEnc = WorldSystem.instance.encounterManager.currentEncounter;
            GridDirection dir = currentTile.exitEncToDirection[currentEnc];
            if (currentEnc != null && GetTile(currentTile.coord + dir) is HexTile targetTile)
            {
                choosableTiles.Add(targetTile);
                targetTile.directionEntry = dir.GetOpposing();
            }

            animator.SetBool("IsPlaying", false);
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
        foreach(HexTile tile in choosableTiles)
            tile.tileState = TileState.InactiveHighlight;
    }

    public void ExitPlacement()
    {
        currentTile.tileState = TileState.Current;
        hexMapController.enableInput = true;
        animator.SetBool("Confirm", true);
    }

    void CreateRow(int row)
    {
        if(row == 0) AddTile(Vector3Int.zero);

        Vector3Int currentCoord = (Vector3Int.zero + GridDirection.SouthWest) * row;
        foreach (GridDirection dir in GridDirection.Directions)
            for (int i = 0; i < row; i++)
                AddTile(currentCoord += dir);
    }

    public void SetButtonsInteractable(bool val)
    {
        buttonRotateConfirm.interactable = val;
        buttonRotateLeft.interactable = val;
        buttonRotateRight.interactable = val;
    }

    public void ButtonRotate(bool clockwise)
    {
        if (currentTile != null) currentTile.RotateTile(clockwise);
    }

    public bool TilePlacementValid(HexTile tile) => tile.availableDirections.Contains(tile.directionEntry);

    public HexTile GetTile(Vector3Int cellCoordinate) => tiles.ContainsKey(cellCoordinate) ? tiles[cellCoordinate] : null;
    HexTile GetRandomTile(int row = 0)
    {
        if (row == 0) return GetTile(Vector3Int.zero);

        List<HexTile> freeTiles = tiles.Values.Except(completedTiles).ToList();
        return freeTiles[Random.Range(0, freeTiles.Count)];
    }

    List<HexTile> GetTilesAtRow(int row)
    {
        if (row == 0) return new List<HexTile>() {GetTile(Vector3Int.zero)};
        List<HexTile> retList = new List<HexTile>();
        
        Vector3Int currentCoord = (Vector3Int.zero + GridDirection.SouthWest) * row;
        foreach (GridDirection dir in GridDirection.Directions)
            for(int i = 0; i < row; i++)
                retList.Add(GetTile(currentCoord += dir));

        return retList;
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
        
        GameObject obj = Instantiate(HexTilePrefab, CellPosToWorldPos(coord), transform.rotation, tileParent);
        obj.name = string.Format("Tile_{0}_{1}_{2}", coord.x, coord.y, coord.z);
    
        HexTile tile = obj.GetComponent<HexTile>();
        tile.coord = coord;
        tile.Init();
        tile.type = (TileType)tileTypes.GetValue(Random.Range(0, tileTypes.Length));

        tiles[coord] = tile;

        if (coord == Vector3.zero)
            world.encounterManager.GenerateFirstHexEncounters(tile);

        SetRandomTileImage(tile);
        tile.tileState = TileState.Inactive;
        tile.ContentVisible(false);
        tile.transform.localScale = Vector3.zero;

        return tile;
    }

    public void ReportEncounter()
    {
        if (finishedEncounterToReport is null) return;
        EventManager.EncounterCompleted(finishedEncounterToReport);
    }
}