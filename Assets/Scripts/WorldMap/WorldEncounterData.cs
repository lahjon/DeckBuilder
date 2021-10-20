using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWorldEncounterData", menuName = "CardGame/WorldEncounterData")]
public class WorldEncounterData : ScriptableObject
{
    public string worldEncounterName;
    public EncounterDifficulty difficulty;
    public RewardStruct rewardStruct;
    public ConditionData clearCondition;
    public WorldEncounterType type;
    public WorldEncounterData[] unlockableEncounters;

    public string Description; 

    public List<WorldEncounterSegmentData> SegmentDatas = new List<WorldEncounterSegmentData>();
}
