using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class ScenarioManager : Manager
{
    //public float hexScale = 0.392f;
    public float hexScale = 0.365f;
    public List<TileGraphics> tileGraphics;
    public PlayerPawn playerPawn;
    public Transform tileParent;
    public GameObject hexTilePrefab, hexTileBlockedPrefab, encounterPrefab;
    public GridSelector gridSelector;
    public GameObject content;
    public Animator animator;
    public bool initialized;
    public Dictionary<Vector3Int, HexTile> tiles = new Dictionary<Vector3Int, HexTile>();
    public HexTile currentTile;
    public float tileSize, tileGap;
    public int width;
    public List<EncounterDataCombat> allCombatEncounters = new List<EncounterDataCombat>();
    public List<EncounterDataCombat> availableCombatEncounters = new List<EncounterDataCombat>();
    public List<EncounterDataCombat> completedCombatEncounters = new List<EncounterDataCombat>();
    public List<EncounterDataRandomEvent> allChoiceEncounters = new List<EncounterDataRandomEvent>();
    public List<EncounterDataRandomEvent> availableChoiceEncounters = new List<EncounterDataRandomEvent>();
    public List<EncounterDataRandomEvent> completedChoiceEncounters = new List<EncounterDataRandomEvent>();
    public ScenarioData scenarioData;
    public static bool ControlsEnabled
    {
        get => _mouseInputEnabled && _cameraMovementEnabled;
        set
        {
            _mouseInputEnabled = value;
            _cameraMovementEnabled = value;
        }
    }
    static bool _mouseInputEnabled;
    public static bool MouseInputEnabled
    {
        get => _mouseInputEnabled;
        set => _mouseInputEnabled = value;
    }
    static bool _cameraMovementEnabled;
    public static bool CameraMovementEnabled
    {
        get => _cameraMovementEnabled;
        set => _cameraMovementEnabled = value;
    }
    int _actionPoints;
    public int ActionPoints
    {
        get => _actionPoints;
        set
        {
            _actionPoints = value;
            world.hudManager.actionPointsText.text = _actionPoints.ToString();
        }
    }
    protected override void Awake()
    {
        base.Awake();
        world.scenarioManager = this;
    }
    protected override void Start()
    {
        base.Start();
        CreateGrid();
    } 

    public void ButtonRest()
    {
        ActionPoints += Random.Range(1,4);
    }

    void CreateGrid()
    {
        for (int i = 0; i <= width; i++)
            CreateRow(i);
        List<Encounter> encountersToOptimize = new List<Encounter>();
        foreach (HexTile tile in tiles.Values)
        {
            tile.AssignNeighboors();
            if (tile.coord != Vector3Int.zero && !tile.Blocked && Random.Range(0, 10) < 2)
            {
                encountersToOptimize.Add(tile.AddEncounter());
            }
        }
        HexOptimizer optimizer = new HexOptimizer();
        optimizer.SetEncounters(encountersToOptimize);
        optimizer.Run();
        ResetEncounters();

        playerPawn.currentTile = tiles.Values.FirstOrDefault(x => x.coord == Vector3Int.zero);

        ActionPoints = 20;
        ControlsEnabled = true;
    }
    public bool RequestActionPoints(int amount)
    {
        if (amount <= ActionPoints)
        {
            ActionPoints = ActionPoints - amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void GenerateMap()
    {
        if (!initialized) StartCoroutine(CreateMap());
    }

    IEnumerator CreateMap()
    {
        yield return null;
    }
    public HexTile GetTile(Vector3Int cellCoordinate) => tiles.ContainsKey(cellCoordinate) ? tiles[cellCoordinate] : null;

    void CreateRow(int row)
    {
        if(row == 0) AddTile(Vector3Int.zero).Revealed = true;

        Vector3Int currentCoord = (Vector3Int.zero + GridDirection.SouthWest) * row;
        foreach (GridDirection dir in GridDirection.Directions)
            for (int i = 0; i < row; i++)
            {
                if (row <= 2)
                    AddTile(currentCoord += dir).Revealed = true;
                
                else
                    AddTile(currentCoord += dir, true);
            }
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
        float z = height * coord.z * -1;

        return new Vector3(x, transform.position.y, z);
    }

    public HexTile AddTile(Vector3Int coord, bool randomBlocked = false)
    {
        System.Array tileTypes = System.Enum.GetValues(typeof(TileType));

        if (coord.x + coord.y + coord.z != 0)
        {
            Debug.LogWarning("Invalid coord");
            return null;
        }

        GameObject obj;
        if (randomBlocked && Random.Range(0, 10) < 2)
            obj = Instantiate(hexTileBlockedPrefab, CellPosToWorldPos(coord), Quaternion.identity, tileParent);
        else
            obj = Instantiate(hexTilePrefab, CellPosToWorldPos(coord), Quaternion.identity, tileParent);

        obj.name = string.Format("Tile_{0}_{1}_{2}", coord.x, coord.y, coord.z);
    
        HexTile tile = obj.GetComponent<HexTile>();
        tile.coord = coord;
        tile.Init();
        tile.type = (TileType)tileTypes.GetValue(Random.Range(1, tileTypes.Length));

        tiles[coord] = tile;

        // if (coord == Vector3.zero)
        //     world.encounterManager.GenerateFirstHexEncounters(tile);

        //tile.tileState = TileState.Inactive;
        tile.ContentVisible(false);
        tile.transform.localScale = Vector3.one * hexScale;

        return tile;
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

}

[System.Serializable]
public class TileGraphics
{
    public GameObject tileMesh;
    public TileType tileType;
}