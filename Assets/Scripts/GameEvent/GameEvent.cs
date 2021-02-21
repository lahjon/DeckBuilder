using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class GameEvent : MonoBehaviour
{
    int id;
    GameEventStatus gameEventStatus;
    GameEvent nextGameEvent;

    protected WorldSystem world;
    protected virtual void Start()
    {
        world = WorldSystem.instance;
    }
    public virtual void StartEvent()
    {
        
    }

    public virtual void EndEvent()
    {
        DestroyImmediate(this);
    }
}