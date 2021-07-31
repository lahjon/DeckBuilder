using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapManager : Manager
{
    public Canvas canvas;
    public int actIndex;
    public List<string> actNames = new List<string>();
    //public Sprite[] worldEncounterIcons;
    public GameObject worldEncounterPrefab;
    public Transform encounterParent;
    public List<WorldEncounter> worldEncounters;
    public WorldEncounterTooltip worldEncounterTooltip;
    protected override void Awake()
    {
        base.Awake();
        world.worldMapManager = this;
        for (int i = 0; i < encounterParent.childCount; i++)
        {
            worldEncounters.Add(encounterParent.GetChild(i).GetComponent<WorldEncounter>());
        }
        worldEncounters.ForEach(x => x.BindData());
    }
    public void OpenMap()
    {
        UpdateMap();
        canvas.gameObject.SetActive(true);
    }

    public void CloseMap()
    {
        canvas.gameObject.SetActive(false);
    }

    public void ButtonEnterTown()
    {
        WorldStateSystem.SetInWorldMap(false);
    }
    void UpdateMap()
    {
        // if new act is unlocked, update all the maps
    }
}
