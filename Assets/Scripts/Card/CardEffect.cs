using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardEffect
{
    public EffectType Type;
    public int Value;
    public int Times;
    public CardTargetType Target;

    public CardEffect() { }

    public CardEffect(EffectType type, int value, int times, CardTargetType cardTargetType = CardTargetType.EnemySingle)
    {
        Type = type;
        Value = value;
        Times = times;
        Target = cardTargetType;
    }

    public static CardEffect operator+(CardEffect a, CardEffect b)
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

        CardEffect effect = new CardEffect(a.Type, a.Value*a.Times + b.Value*b.Times, 1, a.Target); 

        return effect;
    }
}
