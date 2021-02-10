using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterTown : Encounter
{

    void Start()
    {
        //UpdateEncounter();
    }

    public override void UpdateEncounter()
    {
        encounterType = encounterData.type;

        isVisited = isClicked = encounterData.isVisited;
        UpdateIcon();
        SetNormalMat();
    }

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

    public override void ButtonPress()
    {
        if(WorldSystem.instance.worldState == WorldState.Town)
        {
            switch (this.encounterType)
            {
                case EncounterType.TownLeave:
                    Debug.Log("Leave Town!");
                    SetIsVisited();
                    WorldSystem.instance.townManager.LeaveTown();

                    break;
                
                case EncounterType.TownPray:
                    Debug.Log("Pray in Town!");
                    SetIsVisited();
                    WorldSystem.instance.townManager.EnterPray();
                    break;
                
                case EncounterType.TownShop:
                    Debug.Log("Enter Town Shop!");
                    SetIsVisited();
                    WorldSystem.instance.townManager.EnterShop();
                    break;

                case EncounterType.TownTavern:
                    Debug.Log("Enter Town Pray!");
                    
                    SetIsVisited();
                    WorldSystem.instance.townManager.EnterPray();
                    break;
                
                default:
                    isClicked = true;
                    break;
            }
        }
    }

    

    protected override void OnMouseDown()
    {
        // if(WorldSystem.instance.worldState == WorldState.Town)
        // {
        //     Debug.Log(this.encounterType);
        //     switch (this.encounterType)
        //     {
        //         case EncounterType.TownLeave:
        //             Debug.Log("Leave Town!");
        //             SetIsVisited(false);
        //             WorldSystem.instance.townManager.LeaveTown();

        //             break;
                
        //         case EncounterType.TownPray:
        //             Debug.Log("Pray in Town!");
        //             SetIsVisited(false);
        //             WorldSystem.instance.townManager.EnterPray();
        //             break;
                
        //         case EncounterType.TownShop:
        //             Debug.Log("Enter Town Shop!");
        //             SetIsVisited(false);
        //             WorldSystem.instance.townManager.EnterShop();
        //             break;

        //         case EncounterType.TownTavern:
        //             Debug.Log("Enter Town Pray!");
                    
        //             SetIsVisited(false);
        //             WorldSystem.instance.townManager.EnterPray();
        //             break;
                
        //         default:
        //             isClicked = true;
        //             break;
        //     }
        // }
    }
}
