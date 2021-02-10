using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCombat : EventMain
{
    public override bool TriggerEvent()
    {
        WorldSystem.instance.worldStateManager.RemoveState(false);
        WorldSystem.instance.EnterCombat(WorldSystem.instance.uiManager.encounterUI.encounterData.enemyData);
        base.TriggerEvent();
        return true;
    }
}
