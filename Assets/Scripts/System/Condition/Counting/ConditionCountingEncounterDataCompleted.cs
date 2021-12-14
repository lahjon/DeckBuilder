using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConditionCountingEncounterDataCompleted : ConditionCounting
{
   public override void Subscribe() {
        base.Subscribe();
        EventManager.OnEncounterDataCompletedEvent += CheckValid; 
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        EventManager.OnEncounterDataCompletedEvent -= CheckValid;
    }

    public void CheckValid(EncounterData data)
    {
        if (data.name == conditionData.strParameter || conditionData.strParameters.Contains(data.name))
            OnEventNotification();
    }

}