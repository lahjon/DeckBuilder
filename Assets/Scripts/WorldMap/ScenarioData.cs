using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWorldEncounterData", menuName = "CardGame/WorldEncounterData")]
public class ScenarioData : ScriptableObject
{
    public int id;
    public string ScenarioName;
    public ScenarioDifficulty difficulty;
    public RewardStruct rewardStruct;
    public WorldEncounterType type;
    public List<int> unlocksScenarios = new List<int>();

    public string Description; 

    public List<ScenarioSegmentData> SegmentDatas = new List<ScenarioSegmentData>();
}
