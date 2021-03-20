using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArtifactManager : Manager, ISaveableTemp
{
    public List<GameObject> allArtifacts = new List<GameObject>();
    public List<string> allArtifactsName = new List<string>();
    public List<GameObject> allActiveArtifacts = new List<GameObject>();
    public List<string> allActiveArtifactsNames = new List<string>();

    public ArtifactMenu artifactMenu;


    protected override void Awake()
    {
        base.Awake();
        world.artifactManager = this;
        allArtifacts.ForEach(x => allArtifactsName.Add(x.name));
    }
    protected override void Start()
    {
        base.Start();
        Init();
    }

    public void Init()
    {
        allActiveArtifactsNames.ForEach(x => InitArifact(GetArtifactFromName(x)));
    }

    public GameObject GetRandomAvailableArtifact(Rarity rarity = Rarity.None)
    {
        List<GameObject> allAvailableArtifacts = allArtifacts.Except(allActiveArtifacts).ToList();
        
        if (rarity != Rarity.None)
        {
            allAvailableArtifacts = allAvailableArtifacts.Where(x => x.GetComponent<Artifact>().rarity == rarity).ToList();
        }

        if (allAvailableArtifacts.Count > 0)
        {
            int range = Random.Range(0, allAvailableArtifacts.Count);
            GameObject randomArtifact = allAvailableArtifacts[range];
            return randomArtifact;
        }
        return null;
    }

    public GameObject GetRandomActiveArtifact()
    {
        if (allActiveArtifacts.Count > 0)
        {
            int range = Random.Range(0, allActiveArtifacts.Count);
            GameObject randomArtifact = allActiveArtifacts[range];
            return randomArtifact;
        }
        return null;
    }
    void InitArifact(GameObject anArtifact)
    {
        if (anArtifact != null)
        {
            Artifact artifact = anArtifact.GetComponent<Artifact>();
            artifact.AddActivity();
            artifactMenu.AddUIArtifact(anArtifact);
            allActiveArtifacts.Add(anArtifact);
        }
    }
    public void AddArifact(GameObject anArtifact)
    {
        if (anArtifact != null && !allActiveArtifactsNames.Contains(anArtifact.name))
        {
            Artifact artifact = anArtifact.GetComponent<Artifact>();
            artifact.AddActivity();
            artifactMenu.AddUIArtifact(anArtifact);
            allActiveArtifacts.Add(anArtifact);

            allActiveArtifactsNames.Add(anArtifact.name);
            world.SaveProgression();
        }
    }


    public void RemoveArtifact(GameObject anArtifact)
    {
        Debug.Log(anArtifact);
        if (anArtifact != null && allActiveArtifactsNames.Contains(anArtifact.name))
        {
            Artifact artifact = anArtifact.GetComponent<Artifact>();
            artifact.RemoveActivity();
            artifactMenu.RemoveUIArtifact(anArtifact);

            allActiveArtifacts.Remove(anArtifact);
            allActiveArtifactsNames.Remove(anArtifact.name);
            world.SaveProgression();
        }
    }

    public GameObject GetArtifactFromName(string artifactName)
    {
        return allArtifacts[allArtifactsName.IndexOf(artifactName)];
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
