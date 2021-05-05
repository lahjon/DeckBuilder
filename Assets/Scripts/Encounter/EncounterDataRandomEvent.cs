using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounterRandomEvent", menuName = "CardGame/EncounterDataRandomEvent")]
public class EncounterDataRandomEvent : EncounterData
{
    [SerializeField]
    public List<EncounterEventType> events = new List<EncounterEventType>(3);

    [TextArea(5, 5)]
    public string description;
    public string choice1;
    public string choice2;
    public string choice3;

    public List<EncounterData> newEncounterData = new List<EncounterData>(3);
    public List<CardData> cardData = new List<CardData>();

}
