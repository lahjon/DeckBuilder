using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardEffectInfo
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


        if(a.Target != b.Target)
        {
            Debug.LogError("Two different targetTypes attempted to be spliced");
            return null;
        }

        CardEffectInfo effect = new CardEffectInfo(a.Type, a.Value*a.Times + b.Value*b.Times, 1, a.Target); 

        return effect;
    }
}
