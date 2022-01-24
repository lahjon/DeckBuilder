using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public GameObject debug, debugBlock, debugTile;
    public MeshCollider meshCollider;
    public TileType _tileType;
    public TileType TileType
    {
        get => _tileType;
        set
        {
            _tileType = value;
            if (Elevation == 1)
            {
                color = HexGrid.instance.meshColors[(int)_tileType];
            }
        }
    }
    public bool Reachable
    {
        get => Elevation == 1 && !Blocked;
    }
    bool _blocked;
    public bool Blocked
    {
        get => _blocked;
        set
        {
            _blocked = value;
            debugBlock.SetActive(_blocked);
            debugTile.SetActive(!_blocked);
        }
    }
    public bool EncounterTile;
    int _distance;
    public int Distance
    {
        get => _distance;
        set => _distance = value;
    }
    public HexCell PathFrom { get; set; }
    public int SearchHeuristic { get; set; }
    public int SearchPriority => _distance + SearchHeuristic;
    public int SearchPhase { get; set; }
    public HexCell NextWithSamePriority { get; set; }
    public Color color;
    int _elevation;
    public int Elevation
    {
        get => _elevation;
        set
        {
            _elevation = value;
            if (_elevation < 0)
            {
                color = HexGrid.instance.meshColors[1];
            }
            else if (_elevation == 0)
            {
                color = HexGrid.instance.meshColors[2];
            }
            else if (_elevation == 1)
            {
                color = HexGrid.instance.meshColors[3];
            }
            else if (_elevation == 2)
            {
                color = HexGrid.instance.meshColors[4];
            }
        }
    }
    public bool IsUnderwater => Elevation < 1;
    public static HexGrid hexGrid;
    public HexCell[] neighbours = new HexCell[6];
    public Encounter encounter;
    public static ScenarioManager scenarioManager;
    public static BuildManager buildManager;
    public BuildingOverworld building;
    void Awake()
    {
        meshCollider = gameObject.AddComponent<MeshCollider>();
        if (hexGrid == null) hexGrid = HexGrid.instance;
        if (scenarioManager == null) scenarioManager = WorldSystem.instance.scenarioManager;
        if (buildManager == null) buildManager = WorldSystem.instance.buildManager;
    }
    void Update() 
    {
        
    }
    void OnMouseEnter()
    {
        if (BuildManager.Following)
        {
            if (Reachable)
                buildManager.FollowGhost(Position);
            else
                buildManager.HideGhost();
        }
        else
        {
            if (Reachable)
                hexGrid.tileSelector.Show(this);
        }
    }
    void OnMouseExit()
    {
        hexGrid.tileSelector.Hide();
    }

    void OnMouseDown()
    {
        if (!BuildManager.Following)
        {
            if (hexGrid.FindPath(hexGrid.currentCell, this, true) && ScenarioManager.ControlsEnabled && (encounter == null || scenarioManager.RequestActionPoints(encounter.actionPointCost)))
            {
                hexGrid.playerPawn.MoveToLocation(this);
                Debug.Log("Mouse Up");
            } 
        }
        else
        {
            buildManager.PlaceBuilding(this);
        }
    }

    public void EnableHighlight()
    {
        debug.SetActive(true);
    }
    public void DisableHighlight()
    {
        debug.SetActive(false);
    }
    public void StartEncounter()
    {
        if (encounter != null)
        {
            encounter.StartEncounter();
            if (encounter.consumable)
            {
                Destroy(encounter.gameObject);
                encounter = null;
            }
        }
    }
    public Encounter AddEncounter()
    {
        if (encounter == null)
        {
            encounter = Instantiate(scenarioManager.hexGridOverworld.encounterPrefab, transform).GetComponent<Encounter>();
            encounter.Init();
            return encounter;
        }
        return null;
    }
    public Vector3 Position
    {
        get => new Vector3(transform.localPosition.x, transform.localPosition.y + (Elevation * HexMetrics.elevationStep), transform.localPosition.z);
    }
    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbours[(int)direction];
    }
    public void SetNeighbour(HexDirection direction, HexCell cell)
    {
        neighbours[(int)direction] = cell;
        cell.neighbours[(int)direction.Opposite()] = this;
    }
    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(Elevation, neighbours[(int)direction].Elevation);
    }
    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(Elevation, otherCell.Elevation);
    }
}