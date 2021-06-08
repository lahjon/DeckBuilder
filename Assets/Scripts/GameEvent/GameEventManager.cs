using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameEventManager : Manager
{
    public GameEvent currentGameEvent;
    public GameObject gameEventObject;
    List<string> allGameEvents = new List<string>();
    protected override void Awake()
    {
        base.Awake();
        world.gameEventManager = this;
    }
    void CreateEvent(string eventName)
    {
        currentGameEvent = (GameEvent)gameObject.AddComponent(System.Type.GetType(eventName));
    }
    public void StartEvent(string eventName)
    {
        if (currentGameEvent != null) Destroy(currentGameEvent);
        if (string.IsNullOrEmpty(eventName)) return;

        CreateEvent(eventName);
        currentGameEvent.StartGameEvent();
    }
}
