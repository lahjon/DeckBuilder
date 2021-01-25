using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Encounter", menuName = "Encounter")]
public class EncounterData : ScriptableObject
{
public new string name;
public EncounterType type;
public int tier;
public List<EncounterOutcome> encounterOutcome;
public List<EnemyData> enemyData;
public EncounterUI encounterUI;
public bool isVisited = false;

[TextArea(15,20)]
public string description;
}
