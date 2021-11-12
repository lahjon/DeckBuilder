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
    public static void CreateEvent(GameEventStruct gameEventStruct)
    {
        if (InstanceObject(string.Format("GameEvent{0}", gameEventStruct.type.ToString())) is GameEvent gameEvent)
        {
            gameEvent.world = world;
            gameEvent.gameEventStruct = gameEventStruct;
            gameEvent.TriggerGameEvent();
        }
    }

    static GameEvent InstanceObject(string aName)
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

    public bool ValidateStruct()
    {
        bool result = false;
        try
        {
            switch (type)
            {
                case GameEventType.None:
                    break;
                case GameEventType.Custom:
                    break;
                case GameEventType.HighlightBuilding:
                    parameter.ToEnum<BuildingType>();
                    value.ToBool();
                    break;
                case GameEventType.GetReward:
                    parameter.ToEnum<RewardType>();
                    break;
                case GameEventType.ToggleWorldMap:
                    value.ToBool();
                    break;
                case GameEventType.UnlockScenario:
                    int.Parse(value);
                    break;
                case GameEventType.TriggerReward:
                    break;

                default:
                    break;
            }
            result = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception Message: " + ex.Message);
        }
        return result;
    }

}
