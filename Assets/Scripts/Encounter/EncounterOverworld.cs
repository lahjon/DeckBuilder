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
        
        if(gameObject.GetComponent<Encounter>() == encounterManager.overworldEncounters[0])
        {
            SetIsVisited();
        }
        encounterType = encounterData.type;

        isVisited = isClicked = encounterData.isVisited;
        UpdateIcon();
    }

    protected override bool CheckViablePath(Encounter anEncounter)
    {
        return encounterManager.currentEncounter.neighbourEncounters.Contains(anEncounter);
    }

    public override void ButtonPress()
    {
        if(!isVisited && CheckViablePath(this) && !isClicked && WorldStateSystem.instance.currentWorldState == WorldState.Overworld)
        {
            switch (this.encounterType)
            {
                case EncounterType.OverworldCombatNormal:
                    StartCoroutine(SetVisited(() => WorldStateSystem.SetInCombat(true), this));

                    break;
                
                case EncounterType.OverworldCombatElite:
                    StartCoroutine(SetVisited(() => WorldStateSystem.SetInCombat(true), this));

                    break;
                
                case EncounterType.OverworldCombatBoss:
                    StartCoroutine(SetVisited(() => WorldStateSystem.SetInCombat(true), this));

                    break;

                case EncounterType.OverworldShop:
                    StartCoroutine(SetVisited(() => WorldStateSystem.SetInShop(true), this));

                    break;

                case EncounterType.OverworldRandomEvent:
                    WorldSystem.instance.uiManager.encounterUI.encounterData = this.encounterData;
                    StartCoroutine(SetVisited(() => WorldStateSystem.SetInEvent(true), this));
                    
                    break;
                
                default:
                    isClicked = true;
                    break;
            }
        }
    }
}
