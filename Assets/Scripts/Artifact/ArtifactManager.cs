using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArtifactManager : Manager, ISaveableTemp
{
    public List<ArtifactData> allArtifacts {get => DatabaseSystem.instance.arifactDatas;}
    List<int> allArtifactsIds = new List<int>();
    public List<int> allActiveArtifactsIds = new List<int>();
    public HashSet<int> allUnavailableArtifactsNames = new HashSet<int>();

    public ArtifactMenu artifactMenu;


    protected override void Awake()
    {
        base.Awake();
        world.artifactManager = this;
        
    }
    protected override void Start()
    {
        base.Start();
        Init();
    }

    public void Init()
    {
        allArtifacts.ForEach(x => allArtifactsIds.Add(x.itemId));
        allActiveArtifactsIds.ForEach(x => AddArtifactFromLoad(x));
    }

    public ArtifactData GetSpecficArtifact(int artifactId)
    {
        ArtifactData data = allArtifacts[allArtifactsIds.IndexOf(allArtifacts.FirstOrDefault(x => x.itemId == artifactId).itemId)];
        allUnavailableArtifactsNames.Add(data.itemId);
        return data;
    }

    public ArtifactData GetRandomAvailableArtifact()
    {
        List<int> availbleArtifacts = allArtifactsIds.Except(allActiveArtifactsIds).Except(allUnavailableArtifactsNames).ToList();
        //List<string> availbleArtifacts = allArtifactsNames;

        if (availbleArtifacts.Count <= 0)
        {
            // TODO: better handling for when no artifacts left
            availbleArtifacts = allArtifactsIds;
            //return null;
        }

        int artifactId = availbleArtifacts[Random.Range(0, availbleArtifacts.Count)];

        allUnavailableArtifactsNames.Add(artifactId);

        return allArtifacts[allArtifactsIds.IndexOf(artifactId)];
    }

    public int GetRandomActiveArtifact()
    {
        if (allActiveArtifactsIds.Any())
        {
            int range = Random.Range(0, allActiveArtifactsIds.Count);
            return allActiveArtifactsIds[range];
        }
        return -1;
    }
    public void AddArtifact(int artifactId)
    {
        if (artifactId > -1 && !allActiveArtifactsIds.Contains(artifactId))
        {
            if (GetArtifactFromId(artifactId) is ArtifactData artifactData)
            {
                Artifact newArtifact = artifactMenu.AddUIArtifact(artifactData);
                allActiveArtifactsIds.Add(artifactId);
            }
        }
    }

    public void AddArtifactFromLoad(int artifactId)
    {
        if (artifactId > -1)
        {
            if (GetArtifactFromId(artifactId) is ArtifactData artifactData)
            {
                Artifact newArtifact = artifactMenu.AddUIArtifact(artifactData);
                //ItemEffectManager.CreateItemEffect(artifactData.itemEffectStruct, artifactData.itemName);
            }
        }
    }


    public void RemoveArtifact(int artifactId)
    {
        foreach (Artifact artifact in artifactMenu.allUIArtifacts)
        {
            if (artifact.id == artifactId)
            {
                artifact.itemEffect.DeRegister();
                artifactMenu.RemoveUIArtifact(artifact);
                allActiveArtifactsIds.Remove(artifactId);
                break;
            }
        }
    }

    public ArtifactData GetArtifactFromId(int artifactId)
    {
        return allArtifacts[allArtifactsIds.IndexOf(artifactId)];
    }
    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        allActiveArtifactsIds = a_SaveData.allActiveArtifactsNames;
    }

    public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    {
        a_SaveData.allActiveArtifactsNames = allActiveArtifactsIds;
    }
}
