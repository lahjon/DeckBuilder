using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardEffectCarrierData
{
    public EffectType Type;
    public string Value;
    public string Times;
    public CardTargetType Target;
    public ConditionStruct conditionStruct;

    public CardComponentExecType execTime;
}
