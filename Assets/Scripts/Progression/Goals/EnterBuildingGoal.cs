using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnterBuildingGoal : ProgressionGoal, IEvents
{
    public BuildingType buildingGoal;

    public EnterBuildingGoal(Progression aProgression, BuildingType aBuildingGoal, int aRequiredAmount)
    {
        Subscribe();
        buildingGoal = aBuildingGoal;
        goalName = string.Format("Enter {0}", aBuildingGoal);
        progression = aProgression;
        requiredAmount = aRequiredAmount;
        currentAmount = 0;
    }
    public void Subscribe()
    {
        EventManager.OnEnterBuildingEvent += EnterBuilding;
    }
    public void Unsubscribe()
    {
         EventManager.OnEnterBuildingEvent -= EnterBuilding;
    }
    public override void End()
    {
        Unsubscribe();
        progression.CheckGoals();
        Debug.Log(string.Format("Goal {0} ended.", goalName));
    }

    void EnterBuilding(BuildingType building)
    {
        if (buildingGoal == building || building == BuildingType.Any)
        {
            currentAmount++;
            Evaluate();
        }
    }
}