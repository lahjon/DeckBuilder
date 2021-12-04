using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ConditionTypeInfo
{
    private string baseText;
    public string GetTextInfo(ConditionData data) => baseText.Replace("<val>", data.numValue.ToString()).Replace("<param>",data.strParameter);
    public Func<ConditionData, bool> conditionChecker;

    public static ConditionTypeInfo CardsPlayedAtLeast = new ConditionTypeInfo { conditionChecker = CheckCardsPlayedAtLeast, baseText = "If you played at least <val> cards" };
    public static ConditionTypeInfo CardsPlayedAtMost = new ConditionTypeInfo { conditionChecker = CheckCardsPlayedAtMost, baseText = "If you played less than <val> cards" };
    public static ConditionTypeInfo LastCardPlayedTurnType = new ConditionTypeInfo { conditionChecker = CheckLastTypePlayedThisTurn, baseText = "If the last card you played was <param>" };
    public static ConditionTypeInfo OptionalCardCostPaid = new ConditionTypeInfo { conditionChecker = CheckOptionalCardCostPaid, baseText = "(Optional)" };
    public static ConditionTypeInfo NotConfigured = new ConditionTypeInfo { conditionChecker = (ConditionData data) => { return false; }, baseText = "This Conditiontype is not configured for anything other than CountingCondition" };

    private static Dictionary<ConditionType, ConditionTypeInfo> ConditionInfos = new Dictionary<ConditionType, ConditionTypeInfo> {
        {ConditionType.CardsPlayedAtLeast, CardsPlayedAtLeast},
        {ConditionType.CardsPlayedAtMost, CardsPlayedAtMost},
        {ConditionType.LastCardPlayedTurnType, LastCardPlayedTurnType},
        {ConditionType.OptionalCardCostPaid, OptionalCardCostPaid},
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
        return CombatSystem.instance.cardsPlayedThisTurn.Count >= conditionStruct.numValue;
    }

    public static bool CheckCardsPlayedAtMost(ConditionData conditionStruct)
    {
        return CombatSystem.instance.cardsPlayedThisTurn.Count < conditionStruct.numValue;
    }

    public static bool CheckLastTypePlayedThisTurn(ConditionData conditionStruct)
    {
        if (CombatSystem.instance.cardsPlayedThisTurn.Count < 1) return false;
        CardType cardType;
        Enum.TryParse(conditionStruct.strParameter, out cardType);
        return CombatSystem.instance.cardsPlayedThisTurn[CombatSystem.instance.cardsPlayedThisTurn.Count - 1].cardType == cardType;
    }

    public static bool CheckOptionalCardCostPaid(ConditionData conditionStruct)
    {
        return CombatSystem.instance.InProcessCard is Card card ? card.cost.optionalPaid : false;
    }

}


public enum ConditionType
{
    None,
    KillEnemy,
    ClearTile,
    EnterBuilding,
    WinCombat,
    CardsPlayedAtLeast,
    CardsPlayedAtMost,
    LastCardPlayedTurnType,
    Discharge,
    KillBoss,
    StoryTileCompleted,
    EncounterDataCompleted,
    EncounterCompleted,
    StorySegmentCompleted,
    OptionalCardCostPaid,
    TurnEnded
}


