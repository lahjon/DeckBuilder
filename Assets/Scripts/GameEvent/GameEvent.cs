using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class GameEvent
{
    public WorldSystem world;
    public abstract void TriggerGameEvent(GameEventStruct gameEventStruct);
}