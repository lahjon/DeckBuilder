using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission002 : Mission
{
    protected override void Start()
    {
        name = "Defend the church district!";
        startEvent = "Event002";
        //nextMission = "Mission002";
        overrideMissionGoal = "Kill the boss";
        completed = true;
        AddGoal(new KillEnemyGoal(this, 1, "BossMan", "2001"));
        base.Start();
    }
}