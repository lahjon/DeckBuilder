using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounterRandomEvent", menuName = "CardGame/EncounterDataRandomEvent")]
public class EncounterDataRandomEvent : EncounterData
{
    [TextArea(5, 5)]
    public string description;

    [SerializeField]
    public List<EncounterEventChoice> choices = new List<EncounterEventChoice>();

    public EncounterEventChoice chosenOption;
}
