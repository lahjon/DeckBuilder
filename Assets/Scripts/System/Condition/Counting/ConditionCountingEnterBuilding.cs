using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConditionCountingEnterBuilding : ConditionCounting
{
    public override void Subscribe() {
        base.Subscribe();
        EventManager.OnEnterBuildingEvent += CheckValid; 
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        EventManager.OnEnterBuildingEvent -= CheckValid;
    }

    public void CheckValid(BuildingType buildingType)
    {
        if (string.IsNullOrEmpty(conditionData.strParameter) || buildingType.ToString() == conditionData.strParameter)
            OnEventNotification();
    }

}