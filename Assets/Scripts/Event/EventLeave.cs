using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLeave : EventMain
{

    public override bool TriggerEvent()
    {
        WorldSystem.instance.worldStateManager.RemoveState(false);
        base.TriggerEvent();
        return true;
    }
}
