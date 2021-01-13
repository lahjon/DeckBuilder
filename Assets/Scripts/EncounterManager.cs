using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EncounterManager : MonoBehaviour
{
    public List<Encounter> allEncounters;

    void Awake()
    {
        WorldSystem.instance.encounterManager = this;
    }

    public Vector3 GetStartEncounter()
    {
        return allEncounters[0].transform.position;
    }


}
