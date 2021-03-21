using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardEffect
{
    public EffectType Type;
    public int Value;
    public int Times;
    public CardTargetType Target = CardTargetType.EnemySingle;


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

        CardEffect effect = new CardEffect() { Type = a.Type };
        effect.Times = Mathf.Max(a.Times, b.Times);
        if (a.Times == b.Times)
            effect.Value = a.Value + b.Value;
        else
            effect.Value = Mathf.Max(a.Value, b.Value);

        effect.Target = a.Target;

        return effect;
    }
}
