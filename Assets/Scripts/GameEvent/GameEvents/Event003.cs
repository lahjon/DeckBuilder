using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Event003 : GameEvent
{
    public override void StartGameEvent()
    {
        base.StartGameEvent();
        Debug.Log("I think you just finished your fist objective!");
    }  
}