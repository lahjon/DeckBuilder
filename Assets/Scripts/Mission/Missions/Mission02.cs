using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission02 : Mission
{
    protected override void Start()
    {
        missionName = "Defend the church district!";
        startEvent = "Event02";
        //nextMission = "Mission02";
        //overrideMissionGoal = "Kill the boss";
        completed = true;
        goals.Add(new KillEnemyGoal(2, "BossMan", "2001"));
        base.Start();
    }
}