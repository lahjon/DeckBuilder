using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCombat : EventMain
{
    public override bool TriggerEvent()
    {
        print(WorldSystem.instance.uiManager.encounterUI.encounterData.enemyData);
        WorldSystem.instance.EnterCombat(WorldSystem.instance.uiManager.encounterUI.encounterData.enemyData);
        base.TriggerEvent();
        return true;
    }
}
