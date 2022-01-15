using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexTileOverworld : HexTile
{
    static ScenarioManager scenarioManager;
    public Encounter encounter;
    public Transform encounterParent;
    public Transform roadParent;
    public HexTileFog fog;
    public override bool Traverseable => (!Blocked && Revealed && encounter == null);
    public override bool Revealed
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
    public override void Init()
    {
        if (scenarioManager == null) scenarioManager = WorldSystem.instance.scenarioManager;
    }
    protected override void OnMouseEnter()
    {
        if (Revealed)
            Select();
    }
    protected override void OnMouseExit()
    {
        Deselect();
    }
    protected override void OnMouseUp()
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
    public Encounter AddEncounter()
    {
        encounter = Instantiate(scenarioManager.hexGridOverworld.encounterPrefab, transform).GetComponent<Encounter>();
        encounter.Init();
        return encounter;
    }

    public override void AssignNeighboors()
    {
        foreach (GridDirection dir in GridDirection.Directions)
            if (scenarioManager.hexGridOverworld.GetTile(coord + dir) is HexTile neigh)
                neighbours.Add(neigh);
    }
}