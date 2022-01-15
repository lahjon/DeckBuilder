using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexTile : MonoBehaviour
{
    public int id;
    public Vector3Int coord;
    static ScenarioManager scenarioManager;
    public List<HexTile> neighbours = new List<HexTile>();
    public Encounter encounter;
    public Transform encounterParent;
    public Transform roadParent;
    public TileType type;
    public HexTileFog fog;
    public bool Blocked;
    public bool Traverseable => (!Blocked && Revealed && encounter == null);
    bool _revealed;
    public bool Revealed
    {
        get => _revealed;
        set
        {
            if (value)
            {
                _revealed = value;
                fog.Reveal();
            }
        }
    }
    public int cost;
    public void Init()
    {
        if (scenarioManager == null) scenarioManager = WorldSystem.instance.scenarioManager;
    }
    public void ContentVisible(bool visibility)
    {

    }
    void OnMouseEnter()
    {
        if (Revealed)
            Select();
    }

    void OnMouseExit()
    {
        Deselect();
    }

    public void StartEncounter()
    {
        if (encounter != null)
        {
            encounter.StartEncounter();
            if (encounter.oneTime)
            {
                Destroy(encounter.gameObject);
                encounter = null;
            }
        }
    }

    void OnMouseUp()
    {
        if (!(ScenarioManager.MouseInputEnabled && !Blocked && Revealed && (encounter == null || scenarioManager.RequestActionPoints(encounter.actionPointCost)) && scenarioManager.playerPawn.MoveToLocation(this)))
            scenarioManager.gridSelector.FailAction();
    }

    void Select()
    {
        scenarioManager.gridSelector.Reveal(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z));
    }
    void Deselect()
    {
        scenarioManager.gridSelector.Hide();
    }
    public Encounter AddEncounter()
    {
        encounter = Instantiate(scenarioManager.encounterPrefab, transform).GetComponent<Encounter>();
        encounter.Init();
        return encounter;
    }

    public void AssignNeighboors()
    {
        foreach (GridDirection dir in GridDirection.Directions)
            if (scenarioManager.GetTile(coord + dir) is HexTile neigh)
                neighbours.Add(neigh);
    }
}