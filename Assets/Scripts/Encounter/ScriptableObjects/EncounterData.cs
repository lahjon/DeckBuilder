using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EncounterData : ScriptableObject
{
    public int id;
    public string encounterName;
    public EncounterTag[] encounterTags;
    public int tier;
}
