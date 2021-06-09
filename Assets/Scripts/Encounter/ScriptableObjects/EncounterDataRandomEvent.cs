using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounterRandomEvent", menuName = "CardGame/EncounterDataRandomEvent")]
public class EncounterDataRandomEvent : EncounterData
{
    public bool FindInRandom = true;  

    [TextArea(5, 5)]
    public string description;

    [SerializeField]
    public List<EncounterEventChoice> choices = new List<EncounterEventChoice>();

    public EncounterEventChoice chosenOption;
}
