using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CardEffectCarrier: ICardTextElement
{
    public Card card; 

    public EffectType Type;
    public int Value { get; set; }
    public int Times { get; set; }


    public CardTargetType Target;
    public Condition condition;

    public CardEffectCarrier() { }


    public CardEffectCarrier(EffectType type, int value, int times, CardTargetType target = CardTargetType.EnemySingle, Card card = null)
    {
        this.card = card;
        Type = type;
        Value = value;
        Times = times;
        Target = target;
        condition = new Condition();
    }

    public CardEffectCarrier(CardEffectCarrierData data, Card card, Action OnPreConditionUpdate = null)
    {
        this.card = card;
        Type = data.Type;
        Value = data.Value;
        Times = data.Times;
        Target = data.Target;

        condition = new Condition(data.conditionStruct, OnPreConditionUpdate);
        if (data.conditionStruct.type != ConditionType.None)
        {
            card.registeredHighlightConditions.Add(condition);
        }
    }

    public static CardEffectCarrier operator+(CardEffectCarrier a, CardEffectCarrier b)
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

        return new CardEffectCarrier(a.Type, 
                                  a.Value * a.Times + b.Value * b.Times, 
                                  1, 
                                  (CardTargetType)Mathf.Max((int)a.Target, (int)b.Target)
                                  ); 
    }

    public string GetElementText()
    {
        if (Value == 0) return null;
        string retString = condition.GetTextCard();
       
        retString += Type.ToString() + " ";
        if (Type == EffectType.Damage)
            retString += CardVisual.strDamageCode;
        else if (Type == EffectType.Block)
            retString += CardVisual.strBlockCode;
        else
            retString += Value.ToString();

        if (Times != 1) retString += " " + Times + " times";
        if ((Type == EffectType.Damage && Target != CardTargetType.EnemySingle) || (Type == EffectType.Block && Target != CardTargetType.Self)) retString += " " + Target.ToString();

        return retString;
    }

    public string GetElementToolTip()
    {
       if(Type == EffectType.Damage || (Type == EffectType.Block && Value == 0)) return null;
       return Type.GetToolTip();
    }
}
