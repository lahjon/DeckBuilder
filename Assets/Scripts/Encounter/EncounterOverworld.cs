using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterOverworld : Encounter
{

    public override void UpdateEncounter()
    {
        
        if(gameObject.GetComponent<Encounter>() == WorldSystem.instance.encounterManager.overworldEncounters[0])
        {
            SetIsVisited(false);
        }
        else
        {
            SetNormalMat();
        }
        encounterType = encounterData.type;
        encounterUI = encounterData.encounterUI;

        isVisited = isClicked = encounterData.isVisited;
        UpdateIcon();
    }

    protected override bool CheckViablePath(Encounter anEncounter)
    {
        foreach (Encounter x in WorldSystem.instance.encounterManager.currentEncounter.neighbourEncounters)
        {
            if(x == anEncounter)
                return true;
        }
        return false;
    }

    protected override void OnMouseOver()
    {
        
        if(!isVisited && WorldSystem.instance.worldState == WorldState.Overworld)
        {
            if(!highlighted)
                SetHighlightedMat();
            highlighted = true;
        }

    }

    protected override void OnMouseDown()
    {
        if(!isVisited && CheckViablePath(this) && !isClicked && WorldSystem.instance.worldState == WorldState.Overworld)
        {
            switch (this.encounterType)
            {
                case EncounterType.OverworldCombatNormal:
                    Debug.Log("Enter Combat!");
                    SetIsVisited(false);
                    WorldSystem.instance.EnterCombat();
                    break;
                
                case EncounterType.OverworldCombatElite:
                    Debug.Log("Enter Elite Combat!");
                    SetIsVisited(false);
                    WorldSystem.instance.EnterCombat();
                    break;
                
                case EncounterType.OverworldCombatBoss:
                    Debug.Log("Enter Boss Combat!");
                    SetIsVisited(false);
                    WorldSystem.instance.EnterCombat();
                    break;

                case EncounterType.OverworldShop:
                    WorldSystem.instance.shopManager.shop.gameObject.SetActive(true);
                    WorldSystem.instance.shopManager.shop.RestockShop();
                    SetIsVisited(false);
                    WorldSystem.instance.SwapState(WorldState.Shop);
                    Debug.Log("Enter Shop!");
                    break;
                
                default:
                    isClicked = true;
                    CreateUI();
                    break;
            }
        }
    }
}
