using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MissionManager : Manager
{
    public Mission mission;
    public GameObject missions;
    public MissionUI missionUI;

    protected override void Start()
    {
        base.Start();       
        NewMission("Mission001");

    }
    public void NewMission(string newMission)
    {
        mission = (Mission)missions.AddComponent(Type.GetType(newMission));
    }
    public void ClearMission()
    {
        missionUI.ClearUI(true);
        mission = null;
    }

}