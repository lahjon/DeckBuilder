using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class ScenarioManager : Manager
{
    public HexGridOverworld hexGridOverworld;
    public PlayerPawn playerPawn;
    public GridSelector gridSelector;
    public GameObject content;
    public Animator animator;
    public bool initialized;
    public HexTileOverworld currentTile;
    public List<EncounterDataCombat> allCombatEncounters = new List<EncounterDataCombat>();
    public List<EncounterDataCombat> availableCombatEncounters = new List<EncounterDataCombat>();
    public List<EncounterDataCombat> completedCombatEncounters = new List<EncounterDataCombat>();
    public List<EncounterDataRandomEvent> allChoiceEncounters = new List<EncounterDataRandomEvent>();
    public List<EncounterDataRandomEvent> availableChoiceEncounters = new List<EncounterDataRandomEvent>();
    public List<EncounterDataRandomEvent> completedChoiceEncounters = new List<EncounterDataRandomEvent>();
    public ScenarioCameraController scenarioCameraController;
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
        //GenerateScenario();
        ControlsEnabled = true;
    } 

    void GenerateScenario()
    {
        hexGridOverworld.Init();
        ResetEncounters();

        //playerPawn.currentTile = HexGridOverworld.tiles[Vector3Int.zero];

        ActionPoints = 20;
        ControlsEnabled = true;
    }

    public void ButtonRest()
    {
        ActionPoints += Random.Range(1,4);
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