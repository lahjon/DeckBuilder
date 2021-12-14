using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConditionCountingStorySegmentCompleted : ConditionCounting
{
    public override void Subscribe() {
        base.Subscribe();
        EventManager.OnCompleteStorySegmentEvent += CheckValid; 
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        EventManager.OnCompleteStorySegmentEvent -= CheckValid;
    }

    public void CheckValid(ScenarioSegment segment)
    {
        if (conditionData.strParameters.Contains(segment.data.SegmentName))
            OnEventNotification();
    }

}