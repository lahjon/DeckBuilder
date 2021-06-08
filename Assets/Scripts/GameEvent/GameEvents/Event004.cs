using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Event004 : GameEvent
{
    public override void StartGameEvent()
    {
        base.StartGameEvent();
        Debug.Log("Did you just finish you first conversation?!");
    }  
}