using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnterBuildingGoal : MissionGoal
{
    public BuildingType buildingGoal;

    public EnterBuildingGoal(BuildingType aBuildingGoal)
    {
        Init();
        buildingGoal = aBuildingGoal;
    }
    public override void Init()
    {
        EventManager.OnEnterBuildingEvent += EnterBuilding;
    }
    public override void End()
    {
        EventManager.OnEnterBuildingEvent -= EnterBuilding;
        Debug.Log("removed event");
        base.End();
    }

    void EnterBuilding(BuildingType building)
    {
        if (buildingGoal == building)
        {
            Complete();
        }
    }
}