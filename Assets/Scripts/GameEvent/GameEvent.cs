using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class GameEvent : MonoBehaviour
{
    int id;
    GameEventStatus gameEventStatus;
    GameEvent nextGameEvent;

    protected static WorldSystem world;
    protected virtual void Awake()
    {
        world = WorldSystem.instance;
    }
    public virtual void StartGameEvent()
    {
        
    }

    public virtual void EndEvent()
    {
        DestroyImmediate(this);
    }
}