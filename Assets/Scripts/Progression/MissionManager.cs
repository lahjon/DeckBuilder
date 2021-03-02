using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MissionManager : Manager, ISaveableWorld
{
    public Mission mission;
    public GameObject missions;
    public MissionUI missionUI;
    string missionId;
    protected override void Awake()
    {
        base.Awake();
        world.missionManager = this;
    }
    public void NewMission(string newMission, bool save = true)
    {
        mission = (Mission)missions.AddComponent(Type.GetType(newMission));
        missionId = newMission;
        if (save)
        {
            world.SaveProgression();
        }
    }
    public void ClearMission()
    {
        missionUI.ClearUI(true);
        mission = null;
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.missionId = missionId;
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        if (a_SaveData.missionId != null && a_SaveData.missionId != "")
        {
            NewMission(a_SaveData.missionId, false);
        }
    }
}