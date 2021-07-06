using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class TownInteractable : MonoBehaviour, IToolTipable
{
    public new string name;
    public EncounterDataRandomEvent encounterData;
    public BuildingType buildingType;
    public Building building;
    //public string toolTipDescription;
    public Transform tooltipAnchor;

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
        WorldSystem.instance.toolTipManager.DisableTips();
    }

    public virtual (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
        return (new List<string>{name} , pos);
    }
}