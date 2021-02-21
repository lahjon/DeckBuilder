using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour 
{
    public delegate void EnterBuildingEvent(BuildingType building);
    public static event EnterBuildingEvent OnEnterBuildingEvent;

    public static void EnterBuilding(BuildingType building)
    {
        if(OnEnterBuildingEvent != null)
            OnEnterBuildingEvent(building);
    }
}
