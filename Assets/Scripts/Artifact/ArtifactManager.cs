using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArtifactManager : Manager, ISaveableTemp
{
    public List<ArtifactData> allArtifacts = new List<ArtifactData>();
    List<string> allArtifactsNames = new List<string>();
    public List<string> allActiveArtifactsNames = new List<string>();
    public List<string> allUnavailableArtifactsNames = new List<string>();

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
        allArtifacts.ForEach(x => allArtifactsNames.Add(x.itemName));
        allActiveArtifactsNames.ForEach(x => AddArtifact(x));
    }

    public ArtifactData GetSpecficArtifact(string artifactName)
    {
        return allArtifacts[allArtifactsNames.IndexOf(allArtifacts.FirstOrDefault(x => x.name == artifactName).itemName)];
    }

    public ArtifactData GetRandomAvailableArtifact(bool makeUnvailable = false)
    {
        List<string> availbleArtifacts = allArtifactsNames.Except(allActiveArtifactsNames).Except(allUnavailableArtifactsNames).ToList();

        if (availbleArtifacts.Count <= 0)
        {
            return null;
        }

        string itemName = availbleArtifacts[Random.Range(0, availbleArtifacts.Count)];

        if (makeUnvailable)
            allUnavailableArtifactsNames.Add(itemName);

        Debug.Log(allArtifacts[allArtifactsNames.IndexOf(itemName)]);

        return allArtifacts[allArtifactsNames.IndexOf(itemName)];
    }

    public string GetRandomActiveArtifact()
    {
        if (allActiveArtifactsNames.Count > 0)
        {
            int range = Random.Range(0, allActiveArtifactsNames.Count);
            return allActiveArtifactsNames[range];
        }
        return null;
    }
    public void AddArtifact(string artifactName, bool save = false)
    {
        if (artifactName != null && artifactName.Length > 0 && !allActiveArtifactsNames.Contains(artifactName))
        {
            ArtifactData artifactData = GetArtifactFromName(artifactName);
            GameObject newArtifact = artifactMenu.AddUIArtifact(artifactData);
            allActiveArtifactsNames.Add(artifactName);

            Effect.GetEffect(newArtifact, artifactData.name, true);

            if (save)
            {
                world.SaveProgression();
            }
        }
    }


    public void RemoveArtifact(string artifactName)
    {
        foreach (GameObject artifact in artifactMenu.allUIArtifacts)
        {
            if (artifact.name == artifactName)
            {
                artifact.GetComponent<Effect>().RemoveEffect();
                artifactMenu.RemoveUIArtifact(artifact);
                allActiveArtifactsNames.Remove(artifactName);
                break;
            }
        }
    }

    public ArtifactData GetArtifactFromName(string artifactName)
    {
        return allArtifacts[allArtifactsNames.IndexOf(artifactName)];
    }
    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        allActiveArtifactsNames = a_SaveData.allActiveArtifactsNames;
    }

    public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    {
        a_SaveData.allActiveArtifactsNames = allActiveArtifactsNames;
    }
}
