using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterOverworld : Encounter
{
    private EncounterManager encounterManager;
    [HideInInspector]
    public int mapIndex;
    void Awake()
    {
        encounterManager = WorldSystem.instance.encounterManager;
    }

    public override void UpdateEncounter()
    {
        if (gameObject.GetComponent<Encounter>() == encounterManager.overworldEncounters[0])
        {
            StartCoroutine(SetVisited(() => { }));
        }
        encounterType = encounterData.type;
        UpdateIcon();
    }

    protected override bool CheckViablePath(Encounter anEncounter)
    {
        return encounterManager.currentEncounter.neighbourEncounters.Contains(anEncounter);
    }

    public override void ButtonPress()
    {
        if(selectable)
        {
            if (encounterType == EncounterType.OverworldCombatNormal || encounterType == EncounterType.OverworldCombatElite || encounterType == EncounterType.OverworldCombatBoss)
                StartCoroutine(SetVisited(() => WorldStateSystem.SetInCombat(true), this));
            else if (encounterType == EncounterType.OverworldShop)
                StartCoroutine(SetVisited(() => WorldStateSystem.SetInShop(true), this));
            else if (encounterType == EncounterType.OverworldRandomEvent) {
                WorldSystem.instance.uiManager.encounterUI.encounterData = this.encounterData;
                StartCoroutine(SetVisited(() => WorldStateSystem.SetInEvent(true), this));
            }
        }
    }
}
