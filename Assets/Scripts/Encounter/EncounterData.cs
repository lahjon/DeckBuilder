using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Encounter", menuName = "CardGame/Encounter")]
public class EncounterData : ScriptableObject
{
    public new string name;
    public EncounterType type;
    public int tier;
    [HideInInspector]
    public bool isVisited = false;
    public bool secretCondition = false;
    [SerializeField]
    public List<EncounterEventType> events = new List<EncounterEventType>(3);

    [TextArea(5,5)]
    public string description;
    public string choice1;
    public string choice2;
    public string choice3;

    public List<EncounterData> newEncounterData = new List<EncounterData>(3);
    public List<EnemyData> enemyData = new List<EnemyData>();
    public List<CardData> cardData = new List<CardData>();

    public List<CardEffect> startingEffects = new List<CardEffect>();
    public List<CardActivity> startingActivities = new List<CardActivity>();

}
