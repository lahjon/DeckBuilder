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
    public Func<ConditionData, bool> conditionChecker;

    public static ConditionTypeInfo CardsPlayedAtLeast = new ConditionTypeInfo { conditionChecker = CheckCardsPlayedAtLeast, baseText = "If you played at least <val> cards"};
    public static ConditionTypeInfo CardsPlayedAtMost = new ConditionTypeInfo { conditionChecker = CheckCardsPlayedAtMost, baseText = "If you played less than <val> cards" };
    public static ConditionTypeInfo LastCardPlayedTurnType = new ConditionTypeInfo { conditionChecker = CheckLastTypePlayedThisTurn, baseText = "If the last card you played was <param>" };
    public static ConditionTypeInfo OptionalCardCostPaid = new ConditionTypeInfo { conditionChecker = CheckOptionalCardCostPaid, baseText = "(Optional)" };
    public static ConditionTypeInfo HealthPercentLessThan = new ConditionTypeInfo { conditionChecker = HealthBelowPercent, baseText = "If your life is currently below <val>%" };
    public static ConditionTypeInfo NotConfigured = new ConditionTypeInfo { conditionChecker = (ConditionData data) => { Debug.Log("Not configured");return true; }, baseText = "This Conditiontype is not configured for anything other than CountingCondition" };

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

    public static bool CheckCardsPlayedAtLeast(ConditionData conditionStruct)
    {
        Debug.Log("Checking cards played");
        return CombatSystem.instance.cardsPlayedThisTurn.Count >= conditionStruct.numValue;
    }

    public static bool CheckCardsPlayedAtMost(ConditionData conditionStruct)
    {
        Debug.Log("Checking cards played");
        return CombatSystem.instance.cardsPlayedThisTurn.Count < conditionStruct.numValue;
    }

    public static bool CheckLastTypePlayedThisTurn(ConditionData conditionStruct)
    {
        Debug.Log("Checking cardtype played last");
        if (CombatSystem.instance.cardsPlayedThisTurn.Count < 1) return false;
        CardType cardType;
        Enum.TryParse(conditionStruct.strParameter, out cardType);
        return CombatSystem.instance.cardsPlayedThisTurn[CombatSystem.instance.cardsPlayedThisTurn.Count - 1].cardType == cardType;
    }

    public static bool CheckOptionalCardCostPaid(ConditionData conditionStruct)
    {
        Debug.Log("Checking optional cost paid");
        return CombatSystem.instance.InProcessCard is Card card ? card.cost.optionalPaid : false;
    }

    public static bool HealthBelowPercent(ConditionData conditionStruct)
    {
        Debug.Log(string.Format("Checking health {0},{1}", WorldSystem.instance.characterManager.CurrentHealth, CharacterStats.Health));
        return 100*WorldSystem.instance.characterManager.CurrentHealth / CharacterStats.Health < conditionStruct.numValue;
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
}


