using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class TownInteractable : MonoBehaviour
{
    public new string name;
    public EncounterDataRandomEvent encounterData;
    public BuildingType buildingType;
    public Building building;

    public virtual void StartEncounterEvent()
    {
        WorldSystem.instance.uiManager.encounterUI.encounterData = encounterData;
        WorldStateSystem.SetInEvent(true);
        WorldSystem.instance.uiManager.encounterUI.StartEncounter();
    }
    public virtual void ButtonPress()
    {
        if (encounterData != null)
        {
            StartEncounterEvent();
        }
        if(building != null)
        {
            building.EnterBuilding();
        }
    }
}