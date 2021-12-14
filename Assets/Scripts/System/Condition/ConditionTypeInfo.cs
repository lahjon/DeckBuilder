using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ConditionTypeInfo
{
    private string baseText;
    public string GetTextInfo(ConditionData data) => baseText.Replace("<val>", data.numValue.ToString()).Replace("<param>",data.strParameter);

    public static ConditionTypeInfo CardsPlayedAtLeast = new ConditionTypeInfo {baseText = "If you played at least <val> cards"};
    public static ConditionTypeInfo CardsPlayedAtMost = new ConditionTypeInfo { baseText = "If you played less than <val> cards" };
    public static ConditionTypeInfo LastCardPlayedTurnType = new ConditionTypeInfo {baseText = "If the last card you played was <param>" };
    public static ConditionTypeInfo OptionalCardCostPaid = new ConditionTypeInfo {  baseText = "(Optional)" };
    public static ConditionTypeInfo HealthPercentLessThan = new ConditionTypeInfo { baseText = "If your life is currently below <val>%" };
    public static ConditionTypeInfo NotConfigured = new ConditionTypeInfo {baseText = "This Conditiontype is not configured for anything other than CountingCondition" };

    private static Dictionary<ConditionType, ConditionTypeInfo> ConditionInfos = new Dictionary<ConditionType, ConditionTypeInfo> {
        {ConditionType.CardsPlayedAtLeast, CardsPlayedAtLeast},
        {ConditionType.CardsPlayedAtMost, CardsPlayedAtMost},
        {ConditionType.LastCardPlayedTurnType, LastCardPlayedTurnType},
        {ConditionType.OptionalCardCostPaid, OptionalCardCostPaid},
        {ConditionType.HealthPercentLessThan, HealthPercentLessThan},
    };

    public static ConditionTypeInfo GetConditionInfo(ConditionType type)
    {
        if (ConditionInfos.ContainsKey(type))
            return ConditionInfos[type];

        else
            return NotConfigured;
    }

    
}


public enum ConditionType
{
    None = 0,
    HealthPercentLessThan = 1,
    CardsPlayedAtLeast = 2,
    CardsPlayedAtMost = 3,
    LastCardPlayedTurnType = 4,
    OptionalCardCostPaid = 5,
    
    KillEnemy = 101,
    ClearTile = 102,
    EnterBuilding = 103,
    WinCombat = 104,
    Discharge = 105,
    KillBoss = 106,
    StoryTileCompleted = 107,
    EncounterDataCompleted = 108,
    EncounterCompleted = 109,
    StorySegmentCompleted = 110,
    EndTurn = 111,
    SpendEnergySpecific = 112,
    CardPlayType = 113
}


