using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Event002 : GameEvent
{
    public override void StartGameEvent()
    {
        base.StartGameEvent();
        world.dialogueManager.SetDataFromString("DemoStart02");
        world.townManager.worldMapButton.interactable = true;
        Debug.Log("Start Event002!");
    }  
}