using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardEffectInfo // : IEvents
{
    public EffectType Type;
    public int Value;
    public int Times;
    public CardTargetType Target;

    public CardEffectInfo() { }

    public CardEffectInfo(EffectType type, int value, int times, CardTargetType cardTargetType = CardTargetType.EnemySingle)
    {
        Type = type;
        Value = value;
        Times = times;
        Target = cardTargetType;
    }

    public CardEffectInfo Clone()
    {
        return new CardEffectInfo(Type, Value, Times, Target);
    }


    public static CardEffectInfo operator+(CardEffectInfo a, CardEffectInfo b)
    {
        if (a == null)
            return b;
        if (b == null)
            return a;

        if(a.Type != b.Type)
        {
            Debug.LogError("Two different cardEffects attempted to be spliced");
            return null;
        }

        return new CardEffectInfo(a.Type, a.Value * a.Times + b.Value * b.Times, 1, (CardTargetType)Mathf.Max((int)a.Target, (int)b.Target)); ;
    }

}
