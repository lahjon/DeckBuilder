using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MissionManager : Manager, ISaveableWorld
{
    public List<MissionData> clearedMissions = new List<MissionData>();
    public List<MissionData> currentMissions = new List<MissionData>();
    public MissionUI missionUI;
    public Transform missionParent;
    public List<MissionData> allMissionDatas { get => DatabaseSystem.instance.missions; }
    public GameObject missionPrefab;

    protected override void Awake()
    {
        base.Awake();
        world.missionManager = this;
    }

    protected override void Start()
    {
        base.Start();
    }

    public List<Mission> GetAllMission()
    {
        List<Mission> allMissions = new List<Mission>();
        for (int i = 0; i < missionParent.childCount; i++)
        {
            allMissions.Add(missionParent.GetChild(i).GetComponent<Mission>());
        }
        return allMissions;
    }

    public Mission StartMission(MissionData data, bool fromLoad = false)
    {
        if (data == null || clearedMissions.Contains(data)) return null;
        if (!fromLoad && currentMissions.Contains(data)) return null;
        if (!fromLoad) currentMissions.Add(data);

        Mission mission = Instantiate(missionPrefab, missionParent).GetComponent<Mission>();
        mission.name = data.aName;
        for (int i = 0; i < data.addDialogueIdx.Count; i++)
            world.dialogueManager.AddDialogue(data.addDialogueIdx[i]);
        
        mission.StartMission(data);
        return mission;
    }

    public void StartMission(int missionId)
    {
        if (allMissionDatas.FirstOrDefault(x => x.id == missionId) is MissionData data && !clearedMissions.Contains(data) && !currentMissions.Contains(data))
            StartMission(data);
    }
    void RegisterMissions()
    {
        List<MissionData> allMissions = currentMissions.Except(clearedMissions).ToList();
        if (allMissions?.Any() == true) allMissions.ForEach(x => StartMission(x));
    }

    public void AddCompleteMission(Mission mission)
    {
        if (!clearedMissions.Contains(mission.data)) clearedMissions.Add(mission.data);
        if (currentMissions.Contains(mission.data)) currentMissions.Remove(mission.data);

        world.SaveProgression();
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.clearedMissions = clearedMissions.Select(x => x.id).ToList();
        a_SaveData.currentMissions = currentMissions.Select(x => x.id).ToList();

        List<IntListWrapper> allGoals = new List<IntListWrapper>();
        for (int i = 0; i < missionParent.transform.childCount; i++)
        {
            Mission mission = missionParent.transform.GetChild(i).GetComponent<Mission>();
            IntListWrapper innerList = new IntListWrapper();
            innerList.aList = mission.countingConditions.Select(x => x.currentAmount).ToList();
            allGoals.Add(innerList);
        }

        Debug.Log("goal: " + allGoals.Count);
        a_SaveData.currentMissionGoals = allGoals;
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        if (a_SaveData.clearedMissions != null)
        {
            clearedMissions = allMissionDatas.Where(x => a_SaveData.clearedMissions.Contains(x.id)).ToList();
        }
        if (a_SaveData.currentMissions != null)
        {
            currentMissions = allMissionDatas.Where(x => a_SaveData.currentMissions.Contains(x.id)).ToList();
            for (int i = 0; i < currentMissions.Count; i++)
            {
                Mission mission = StartMission(currentMissions[i], true);
                for (int g = 0; g < mission.countingConditions.Count; g++)
                {
                    mission.countingConditions[g].currentAmount = a_SaveData.currentMissionGoals[i][g];
                }
            }
        }
    }
}

