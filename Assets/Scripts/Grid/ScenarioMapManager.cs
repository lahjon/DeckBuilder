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
    public TurnCounter turnCounter;
    public Material undiscoveredMaterial, discoveredMaterial;
    public Dictionary<Vector3Int, HexTile> tiles = new Dictionary<Vector3Int, HexTile>();
    public HexTile currentTile;
    public List<HexTile> completedTiles = new List<HexTile>();
    public List<HexTile> dangerTiles = new List<HexTile>();
    public GridState gridState;
    public GameObject content;
    public ScenarioCameraController hexMapController;
    public int turnTriggerLimit;
    [SerializeField] int _currentTurn;
    public int currentTurn
    {
        get => _currentTurn;
        set
        {
            _currentTurn = value;
            turnCounter.counter--;
            if (currentTurn >= turnTriggerLimit)
            {
                int row = gridWidth - currentTurn + turnTriggerLimit;
                if (row >= 0)
                    dangerTiles.AddRange(GetTilesAtRow(row));
                world.uiManager.UIWarningController.CreateWarning("The void approaches!");
            }
        }
    }
    public float hexScale = 0.392f;
    public bool initialized;
    int furthestRowReached;
    public Transform tileParent, roadParent;
    public TMP_Text conditionText;

    public HashSet<HexTile> choosableTiles = new HashSet<HexTile>();

    [HideInInspector]
    public Encounter finishedEncounterToReport;
    public ObjectiveDisplayer objectiveDisplayer;


    public ScenarioData scenarioData;
    public Scenario scenario;
    
    public int rotateCounter;
    public float rotationAmount;
    public Button buttonRotateLeft, buttonRotateRight, buttonRotateConfirm; 

    protected override void Awake()
    {
        base.Awake();
        world.scenarioMapManager = this;
        animator = GetComponent<Animator>();
    }
    protected override void Start()
    {
        base.Start();
    } 
    


    public void GenerateMap()
    {
        if (!initialized) StartCoroutine(CreateMap());
    }
    public void ButtonCompleteCurrentTile()
    {
        CompleteCurrentTile();
    }
    //public void ButtonConfirm() => currentTile.EndPlacement();
  
    public void DeleteMap()
    {
        initialized = false;
        currentTile = null;
        currentTurn = 0;
        choosableTiles.Clear();
        dangerTiles.Clear();
        tiles.Clear();
        completedTiles.Clear();
        turnCounter.counter = 0;
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
        
        //tile.undiscoveredSpriteRenderer.sprite = inactiveTilesSprite[Random.Range(0, inactiveTilesSprite.Count())];
    }

    IEnumerator CreateMap()
    {
        //content.SetActive(true);
        yield return null;
    }


    public void ExpandMap()
    {
        gridWidth++;
        CreateRow(gridWidth);
        GetTilesAtRow(gridWidth).ForEach(x => 
            x.transform.DOScale(hexScale, 1).SetEase(Ease.InExpo).SetLoops(1, LoopType.Yoyo)
        );
    }

    public void CompleteCurrentTile()
    {
        if (currentTile != null)
        {
            List<int> tileCoords = new List<int>() { currentTile.coord.x, currentTile.coord.y, currentTile.coord.z };
            furthestRowReached = Mathf.Max(furthestRowReached, tileCoords.Select(x => Mathf.Abs(x)).Max());

            Encounter currentEnc = WorldSystem.instance.encounterManager.currentEncounter;
            currentTurn++;
            animator.SetBool("IsPlaying", false);
        }
    }

    public void StartLinkedScenario()
    {
        WorldStateSystem.instance.overrideTransitionType = TransitionType.EnterMap;
        scenarioData = DatabaseSystem.instance.scenarios.Where(s => s.id == scenario.data.linkedScenarioId).FirstOrDefault();
        //ResetEncounters();
        animator.SetBool("InTransition", true);
        Helpers.DelayForSeconds(1f, () => { 
            //hexMapController.ResetCamera();
            
            animator.SetBool("InTransition", false);
            DeleteMap();
            GenerateMap();
        });
    }
    
    // public void AddRandomExit(int row)
    // {
    //     Debug.Log("stranger danger!");
    //     if (row == 0) return;

    //     HexTile tile = GetRandomTile(row);
    //     if (tile == null) 
    //         AddRandomExit(row + 1);
    //     else 
    //         choosableTiles.Add(tile);
    // }
    public void HighlightChoosable()
    {

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
        obj.transform.Rotate(Vector3.forward, 90);
        obj.name = string.Format("Tile_{0}_{1}_{2}", coord.x, coord.y, coord.z);
    
        HexTile tile = obj.GetComponent<HexTile>();
        tile.coord = coord;
        tile.Init();
        tile.type = (TileType)tileTypes.GetValue(Random.Range(1, tileTypes.Length));

        tiles[coord] = tile;

        if (coord == Vector3.zero)
            world.encounterManager.GenerateFirstHexEncounters(tile);

        SetRandomTileImage(tile);
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