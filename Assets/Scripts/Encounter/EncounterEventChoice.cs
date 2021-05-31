using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EncounterEventChoice
{
    public string label;
    public EncounterEventChoiceOutcome outcome;
    public EncounterData newEncounter;
    [SerializeField]
    public List<CardData> cardRewards = new List<CardData>();
}
