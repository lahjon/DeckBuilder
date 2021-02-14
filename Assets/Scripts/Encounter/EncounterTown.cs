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
    }

    protected override bool CheckViablePath(Encounter anEncounter)
    {
        if(anEncounter.encounterData.isVisited)
            return true;
        else
            return false;
    }
    public override void ButtonPress()
    {
        switch (WorldSystem.instance.worldState)
        {
            case WorldState.Town:
            {
                
                switch (this.encounterType)
                {
                    case EncounterType.TownLeave:
                        
                        Debug.Log("Leave Town!");
                        //SetIsVisited();
                        WorldSystem.instance.townManager.LeaveTown();
                        
                        break;
                    
                    case EncounterType.TownPray:

                        Debug.Log("Pray in Town!");
                        //SetIsVisited();
                        WorldSystem.instance.townManager.EnterPray();

                        break;
                    
                    case EncounterType.TownShop:

                        Debug.Log("Enter Town Shop!");
                        //SetIsVisited();
                        WorldSystem.instance.townManager.EnterShop();

                        break;

                    case EncounterType.TownTavern:

                        Debug.Log("Enter Town Pray!");
                        //SetIsVisited();
                        WorldSystem.instance.townManager.EnterPray();

                        break;
                    
                    case EncounterType.TownTownHall:
                        
                        if(!this.isVisited)
                        {
                            Debug.Log("Enter TownHall!");
                            //SetIsVisited();
                            WorldSystem.instance.townManager.EnterTownHall();
                        }

                        break;
                    
                    case EncounterType.TownBarracks:

                        Debug.Log("Enter Barracks!");
                        //SetIsVisited();
                        WorldSystem.instance.townManager.EnterBarracks();

                        break;
                    
                    default:
                        isClicked = true;
                        break;
                }
                break;
            }
            default:
                break;
        }
    }
}
