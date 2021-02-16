using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventMain : MonoBehaviour
{
    /// <returns>bool disable: Returns a bool that is used to disable the UI.</returns>
    public virtual bool TriggerEvent()
    {
        return true;
    }
}
