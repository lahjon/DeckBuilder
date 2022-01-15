using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConditionCountingStoryTileCompleted : ConditionCounting
{
    public override void Subscribe() {
        base.Subscribe();
        EventManager.OnCompleteTileEvent += CheckValid; 
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        EventManager.OnCompleteTileEvent -= CheckValid;
    }

    public void CheckValid(HexTile tile)
    {
        // if (tile.storyId == conditionData.strParameter)
        //     OnEventNotification();
    }

}