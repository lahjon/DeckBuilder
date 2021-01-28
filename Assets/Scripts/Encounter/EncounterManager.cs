using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EncounterManager : MonoBehaviour
{
    public List<GameObject> actEncounters;
    public List<Encounter> overworldEncounters;
    public GameObject townEncounter;
    public Encounter currentEncounter;
    public int encounterTier;
    public Vector3 GetStartPositionEncounter()
    {
        return overworldEncounters[0].transform.position;
    }

    public void UpdateAllOverworldEncounters(int act)
    {
        encounterTier = act;
        overworldEncounters.Clear();
        Transform t = actEncounters[act - 1].transform;
        for (int i = 0; i < t.childCount; i++)
        {   
            Encounter e = t.GetChild(i).gameObject.GetComponent<Encounter>();
            overworldEncounters.Add(e);
            Debug.Log(e);
            e.UpdateEncounter();
        }
    }
    public void UpdateAllTownEncounters(int act)
    {
        encounterTier = act;
        Transform t = townEncounter.transform;
        Debug.Log( t.childCount);
        for (int i = 0; i < t.childCount; i++)
        {   
            Encounter e = t.GetChild(i).gameObject.GetComponent<Encounter>();
            e.UpdateEncounter();
        }
    }
}
