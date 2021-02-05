using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCombat : EventMain
{
    public override bool TriggerEvent()
    {
        print(WorldSystem.instance.uiManager.encounterUITest.encounterData.enemyData);
        WorldSystem.instance.EnterCombat(WorldSystem.instance.uiManager.encounterUITest.encounterData.enemyData);
        base.TriggerEvent();
        return true;
    }
}
