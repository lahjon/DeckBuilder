using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLeave : EventMain
{
    new string name = "Leave"; 

    public override bool TriggerEvent()
    {
        WorldSystem.instance.SwapState(WorldState.Overworld, false);
        base.TriggerEvent();
        return true;
    }
}
