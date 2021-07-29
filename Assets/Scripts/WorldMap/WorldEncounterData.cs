using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWorldEncounterData", menuName = "CardGame/WorldEncounterData")]
public class WorldEncounterData : ScriptableObject
{
    public string worldEncounterName;
    public RewardStruct rewardStruct;
    public WorldEncounterType type;
}
