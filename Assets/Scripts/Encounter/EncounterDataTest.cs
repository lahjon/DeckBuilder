using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Encounter", menuName = "EncounterTest")]
public class EncounterDataTest : ScriptableObject
{
    public new string name;
    public EncounterType type;
    public int tier;
    [HideInInspector]
    public bool isVisited = false;
    public bool secretCondition = false;
    [SerializeField]
    public List<GameObject> events = new List<GameObject>();



    [TextArea(5,5)]
    public string description;
    public string choice1;
    public string choice2;
    public string choice3;

    public List<EnemyData> enemyData = new List<EnemyData>();
    public EncounterDataTest newEncounterData;
    public List<CardData> cardData = new List<CardData>();

}
