using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Event003 : GameEvent
{
    public override void StartEvent()
    {
        base.StartEvent();
        Debug.Log("I think you just finished your fist objective!");
    }  
}