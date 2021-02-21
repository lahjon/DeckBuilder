using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameEventManager : Manager
{
    public GameEvent currentGameEvent;
    public GameObject gameEventObject;
    private List<string> allGameEvents = new List<string>();
    protected override void Start()
    {
        base.Start();
    }
    private void CreateEvent(string eventName)
    {
        
        currentGameEvent = (GameEvent)gameObject.AddComponent(System.Type.GetType(eventName));
    }
    public void StartEvent(string eventName)
    {
        if (currentGameEvent != null)
        {
            DestroyImmediate(currentGameEvent);
        }
        CreateEvent(eventName);
        currentGameEvent.StartEvent();
    }
}
