using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


[System.Serializable]
public class CardEffectCarrier: ICardTextElement
{
    internal Card card; 

    public EffectTypeInfo Type;

    public CardInt Value;
    public CardInt Times;

    internal string description = string.Empty;

    public CardTargetType Target;
    public Condition condition;

    public CardEffectCarrier() { }

    public CardEffectCarrier(EffectType type, int value, int times = 1, CardTargetType target = CardTargetType.EnemySingle, Card card = null)
    {
        this.card = card;
        Type = type.GetEffectTypeStruct();
        Value = new CardInt(value);
        Times = new CardInt(times);
        Target = target;
        condition = new Condition();
        if (Type.Equals(EffectType.Damage)) RegisterDamageComponent();
    }

    public CardEffectCarrier(CardEffectCarrierData data, Card card, Action OnConditionFlip = null)
    {
        this.card = card;
        Type = data.Type.GetEffectTypeStruct();
        Value = CardInt.Factory(data.Value,card, ForceTextRefresh);
        Times = CardInt.Factory(data.Times,card, ForceTextRefresh);
        Target = data.Target;

        condition = new Condition(data.conditionStruct, null, OnConditionFlip );

        if (data.conditionStruct.type != ConditionType.None)
        {
            card.registeredConditions.Add(condition);
            card.registeredSubscribers.Add(condition);
        }

        if (Type.effectType == EffectType.Damage) RegisterDamageComponent();
    }

    public void RegisterDamageComponent()
    {
        if (card is CardCombat c)
            c.OnDamageRecalcEvent += ForceTextRefresh;
    }

    public bool CanAbsorb(CardEffectCarrierData data)
    {
        return
            Type.Equals(data.Type)
            && condition.conditionData.type == data.conditionStruct.type
            && (int)Target <= (int)data.Target
            && (Times.propertyType == data.Times.linkedProp || data.Times.linkedProp == CardLinkablePropertyType.None)
            && (Value.propertyType == data.Value.linkedProp || data.Value.linkedProp == CardLinkablePropertyType.None);
    }

    public void AbsorbModifier(CardEffectCarrierData data)
    {
        Target = data.Target;
        Times.AbsorbModifier(data.Times);
        Value.AbsorbModifier(data.Value);
        description = string.Empty;
    }

    public string GetElementText()
    {
        if (!description.Equals(string.Empty)) return description;
        if (!string.IsNullOrEmpty(Type.txtOverride)) return Type.txtOverride;

        StringBuilder sb = new StringBuilder(100);

        sb.Append(condition.GetTextCard());
        sb.Append(Type.verb);
        sb.Append(" ");

        if (Type.Equals(EffectType.Damage))
        {
            if (Value is CardIntLinkedProperty cip)
                sb.Append(cip.GetTextForValue());
            else
                sb.Append(Helpers.ValueColorWrapper(Value.value + WorldSystem.instance.characterManager.characterStats.GetStat(StatType.Strength), CombatSystem.instance.CalculateDisplayDamage(Value)));
        }
        else
            sb.Append(Value.GetTextForValue());

        sb.Append(String.Format(" <b>{0}</b>",Type.ToString()));

        if (Times != 1) sb.Append(string.Format(" {0} times",Times.GetTextForTimes()));
        if (Target != CardTargetType.Self && Target != CardTargetType.EnemySingle) sb.Append(" " + Target.ToString());

        description = sb.ToString();
        return description;
    }

    public string GetElementToolTip() => Type.toolTipCard;

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

        if(!a.Type.Equals(b.Type))
        {
            Debug.LogError("Two different cardEffects attempted to be spliced");
            return null;
        }

        return new CardEffectCarrier(a.Type.effectType, 
                                  a.Value * a.Times + b.Value * b.Times, 
                                  1, 
                                  (CardTargetType)Mathf.Max((int)a.Target, (int)b.Target)
                                  ); 
    }
}
