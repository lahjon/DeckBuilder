using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MissionManager : Manager, ISaveableWorld
{
    public Mission mission;
    public string currentMissionId = "";
    public GameObject missions;
    public MissionUI missionUI;
    public List<MissionData> missionDatas;
    protected override void Awake()
    {
        base.Awake();
        world.missionManager = this;
    }

    MissionData GetMissionFromString(string missionId)
    {
        
        return missionDatas.FirstOrDefault(x => x.name == missionId);
    }
    public void NewMission(string newMissionId, bool save = true)
    {

        if (missionDatas.FirstOrDefault(x => x.id == newMissionId) is MissionData missionData) 
        {
            mission.StartMission(missionData);
            Debug.Log("Start mission: " + newMissionId);
            currentMissionId = newMissionId;

            if (save) world.SaveProgression();
        }
        else
        {
            Debug.LogWarning("No mission with ID: " + newMissionId);
            return;
        }
    }
    public void ClearMission()
    {
        missionUI.ClearUI(true);
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.currentMissionId = currentMissionId;
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        if (a_SaveData.currentMissionId != null && a_SaveData.currentMissionId != "")
        {
            NewMission(a_SaveData.currentMissionId, false);
        }
    }
}