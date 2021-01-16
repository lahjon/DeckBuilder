using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EncounterManager : MonoBehaviour
{
    public List<Encounter> allEncounters;
    public Encounter currentEncounter;
    public Vector3 GetStartPositionEncounter()
    {
        return allEncounters[0].transform.position;
    }
}
