using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CardEffectCarrier: ICardTextElement
{
    internal Card card; 

    public EffectType Type;

    public CardInt Value;
    public CardInt Times;

    internal string description = string.Empty;

    public CardTargetType Target;
    public Condition condition;

    public CardEffectCarrier() { }

    public CardEffectCarrier(EffectType type, int value, int times = 1, CardTargetType target = CardTargetType.EnemySingle, Card card = null)
    {
        this.card = card;
        Type = type;
        Value = new CardInt(value);
        Times = new CardInt(times);
        Target = target;
        condition = new Condition();
    }

    public CardEffectCarrier(CardEffectCarrierData data, Card card, Action OnPreConditionUpdate = null)
    {
        this.card = card;
        Type = data.Type;
        Value = CardInt.Factory(data.Value,card, ForceTextRefresh);
        Times = CardInt.Factory(data.Times,card, ForceTextRefresh);
        Target = data.Target;

        condition = new Condition(data.conditionStruct, OnPreConditionUpdate);
        if (data.conditionStruct.type != ConditionType.None)
        {
            card.registeredConditions.Add(condition);
            card.registeredSubscribers.Add(condition);
        }
    }

    public virtual string GetElementText()
    {
        if (!description.Equals(string.Empty)) return description;

        if (Value == 0) return null;
        description = condition.GetTextCard();

        description += "<b>" + Type.ToString() + "</b> ";
        if (Type == EffectType.Damage)
            description += CardVisual.strDamageCode;
        else if (Type == EffectType.Block)
            description += CardVisual.strBlockCode;
        else
            description += Value.GetTextForValue();

        if (Times != 1) description += " " + Times + " times";
        if ((Type == EffectType.Damage && Target != CardTargetType.EnemySingle) || (Type == EffectType.Block && Target != CardTargetType.Self)) description += " " + Target.ToString();


        return description;
    }

    public string GetElementToolTip()
    {
       if(Type == EffectType.Damage || (Type == EffectType.Block && Value == 0)) return null;
       return Type.GetToolTip();
    }

    public void ForceTextRefresh()
    {
        if (card is CardCombat c)
        {
            description = string.Empty;
            c.RefreshDescriptionText(true);
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
}
