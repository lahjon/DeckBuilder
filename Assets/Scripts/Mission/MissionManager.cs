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

public float avgFrameRate;
    public void Update()
    {
        avgFrameRate = Time.frameCount / Time.time;
        Debug.Log(avgFrameRate);
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