using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConditionCountingEncounterCompleted : ConditionCounting
{

    public override void Subscribe() {
        base.Subscribe();
        EventManager.OnEncounterCompletedEvent += CheckValid; 
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        EventManager.OnEncounterCompletedEvent -= CheckValid;
    }

    public void CheckValid(Encounter enc)
    {
        ScenarioEncounterType type;
        Enum.TryParse(conditionData.strParameter, out type);
        if (type == enc.encounterType || enc.storyID == conditionData.strParameter)
            OnEventNotification();
    }

}