using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounterRandomEvent", menuName = "CardGame/EncounterDataRandomEvent")]
public class EncounterDataRandomEvent : EncounterData
{
    public EncounterEventLayoutType layoutType = EncounterEventLayoutType.NoImage;
    public bool FindInRandom = true;  

    [TextArea(5, 5)]
    public string description;

    [SerializeField]
    public List<EncounterEventChoice> choices = new List<EncounterEventChoice>();

    [HideInInspector]
    public EncounterEventChoice chosenOption;

    public Sprite art;
}
