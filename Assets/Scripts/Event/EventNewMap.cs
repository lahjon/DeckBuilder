using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNewMap : EventMain
{
    public override bool TriggerEvent()
    {
        WorldSystem.instance.encounterManager.GenerateMap(2,2,4);
        WorldSystem.instance.worldStateManager.AddState(WorldState.Overworld, true);
        return true;
    }
}
