using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardEffectCarrierData : ICardUpgradingData
{
    public StatusEffectType Type;
    public CardIntData Value;
    public CardIntData Times;
    public CardTargetType Target;
    public ConditionData conditionStruct;

    public CardComponentExecType execTime;

}
