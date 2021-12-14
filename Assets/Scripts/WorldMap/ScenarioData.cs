using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWorldEncounterData", menuName = "CardGame/WorldEncounterData")]
public class ScenarioData : ScriptableObject
{
    public int id;
    public int turnTriggerLimit;
    public int minTier;
    public int maxTier;
    public string ScenarioName;
    public ScenarioDifficulty difficulty;
    public RewardNormalStruct rewardStruct;
    public WorldEncounterType type;
    public List<int> unlocksScenarios = new List<int>();
    public EncounterTag[] includeEncounters;
    public EncounterTag[] excludeEncounters;
    public Encounter[] encounters;
    public string DescriptionShort;
    public string Description; 

    public List<ScenarioSegmentData> SegmentDatas = new List<ScenarioSegmentData>();
    public int linkedScenarioId = -1;
}
