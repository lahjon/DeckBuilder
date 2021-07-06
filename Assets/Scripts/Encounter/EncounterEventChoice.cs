using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EncounterEventChoice
{
    public string label;
    public EncounterEventChoiceOutcome outcome;
    public EncounterData newEncounter;
    public List<EncounterEventEffectStruct> effects = new List<EncounterEventEffectStruct>();
}