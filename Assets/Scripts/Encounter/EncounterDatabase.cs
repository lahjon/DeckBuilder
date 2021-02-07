using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounterDatabase", menuName = "Database/Encounter Database")]
public class EncounterDatabase : ScriptableObject
{
    public List<EncounterData> allOverworld;
    public List<EncounterData> bossEncounters;

}