using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EncounterManager : MonoBehaviour
{
    public List<Encounter> allEncounters;
    public Encounter currentEncounter;

    void Start()
    {
        WorldSystem.instance.encounterManager = this;
    }

    public void SetCurrentEncounter(Encounter anEncounter)
    {
        currentEncounter = anEncounter;
    }

    public Vector3 GetStartEncounter()
    {
        return allEncounters[0].transform.position;
    }



}
