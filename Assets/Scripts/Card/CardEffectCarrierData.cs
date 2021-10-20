using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardEffectCarrierData
{
    public EffectType Type;
    public CardIntData Value;
    public CardIntData Times;
    public CardTargetType Target;
    public ConditionData conditionStruct;

    public CardComponentExecType execTime;

}
