using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterTown : Encounter
{
    protected override bool CheckViablePath(Encounter anEncounter)
    {
        if(anEncounter.encounterData.isVisited)
            return true;
        else
            return false;
    }

    protected override void OnMouseOver()
    {
        
        if(!isVisited && WorldSystem.instance.worldState == WorldState.Town)
        {
            if(!highlighted)
                SetHighlightedMat();
            highlighted = true;
        }

    }

    protected override void OnMouseDown()
    {
        if(WorldSystem.instance.worldState == WorldState.Town)
        {
            switch (this.encounterType)
            {
                case EncounterType.TownLeave:
                    Debug.Log("Leave Town!");
                    SetIsVisited(false);
                    WorldSystem.instance.townManager.LeaveTown();
                    break;
                
                case EncounterType.TownPray:
                    Debug.Log("Pray in Town!");
                    SetIsVisited(false);
                    WorldSystem.instance.townManager.EnterPray();
                    break;
                
                case EncounterType.TownShop:
                    Debug.Log("Enter Town Shop!");
                    SetIsVisited(false);
                    WorldSystem.instance.townManager.EnterShop();
                    break;

                case EncounterType.TownTavern:
                    Debug.Log("Enter Town Pray!");
                    SetIsVisited(false);
                    WorldSystem.instance.townManager.EnterPray();
                    break;
                
                default:
                    isClicked = true;
                    CreateUI();
                    break;
            }
        }
    }
}
