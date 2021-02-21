using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MissionManager : Manager
{
    public Mission mission;
    public GameObject missions;

    protected override void Start()
    {
        base.Start();       
        NewMission("Mission01");

    }
    public void NewMission(string newMission)
    {
        mission = (Mission)missions.AddComponent(Type.GetType(newMission));
    }
    public void CheckProgression()
    {
        Debug.Log("checking progression");
        mission.CheckGoals();
    }

}