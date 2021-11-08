using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameEventManager : Manager
{
    protected override void Awake()
    {
        base.Awake();
        world.gameEventManager = this;
    }
    public void CreateEvent(GameEventStruct gameEventStruct)
    {
        if (InstanceObject(string.Format("GameEvent{0}", gameEventStruct.type.ToString())) is GameEvent gameEvent)
        {
            gameEvent.world = world;
            gameEvent.TriggerGameEvent(gameEventStruct);
        }
    }

    GameEvent InstanceObject(string aName)
    {
        if (Type.GetType(aName) is Type type && (GameEvent)Activator.CreateInstance(type, world) is GameEvent gameEvent)
            return gameEvent;
        else
            return null;
    }
}

[System.Serializable]
public struct GameEventStruct
{
    public GameEventType type;
    public string parameter;
    public string value;
    public GameEventStruct(GameEventType aType, string aParm, string aValue)
    {
        type = aType;
        parameter = aParm;
        value = aValue;
    }
}
