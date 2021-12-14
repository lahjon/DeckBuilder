using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


[System.Serializable]
public class StatusEffectCarrier: ICardTextElement, ICardUpgradableComponent
{
    internal Card card; 

    public StatusEffectTypeInfo info;

    public CardInt Value;
    public CardInt Times;

    internal string description = string.Empty;

    public CardTargetType Target;
    public Condition condition;

    HashSet<ModifierType> modifiedTypes = new HashSet<ModifierType>();

    public StatusEffectCarrier() { }

    public StatusEffectCarrier(StatusEffectType type, int value, int times = 1, CardTargetType target = CardTargetType.EnemySingle, Card card = null)
    {
        this.card = card;
        info = StatusEffectTypeInfo.GetEffectTypeInfo(type);
        Value = new CardInt(value);
        Times = new CardInt(times);
        Target = target;
        condition = new ConditionNotConfigured();
        if (info.Equals(StatusEffectType.Damage)) RegisterDamageComponent();
    }

    public StatusEffectCarrier(CardEffectCarrierData data, Card card, Action OnConditionFlip = null)
    {
        this.card = card;
        info = StatusEffectTypeInfo.GetEffectTypeInfo(data.Type);
        Value = CardInt.Factory(data.Value,card, ForceTextRefresh);
        Times = CardInt.Factory(data.Times,card, ForceTextRefresh);
        Target = data.Target;

        condition = Condition.Factory(data.ConditionData, card, null, OnConditionFlip );

        if (data.ConditionData.type != ConditionType.None)
        {
            card.registeredConditions.Add(condition);
            card.registeredSubscribers.Add(condition);
        }

        if (info.type == StatusEffectType.Damage) RegisterDamageComponent();
    }

    public void RegisterDamageComponent()
    {
        if (card is CardCombat c)
            c.OnDamageRecalcEvent += ForceTextRefresh;
    }
    public bool CanAbsorb<T>(T modifier)
    {
        if(modifier is CardEffectCarrierData data)
        {
            return
            info.Equals(data.Type)
            && condition.conditionData.type == data.ConditionData.type
            && (int)Target <= (int)data.Target
            && (data.Value.baseVal == 0 || data.Times.baseVal == Times.baseVal)
            && (Times.propertyType == data.Times.linkedProp || data.Times.linkedProp == CardLinkablePropertyType.None)
            && (Value.propertyType == data.Value.linkedProp || data.Value.linkedProp == CardLinkablePropertyType.None);
        }
        return false;
    }

    public void AbsorbModifier<T>(T modifier)
    {
        if (modifier is CardEffectCarrierData data)
        {
            Target = data.Target;
            if (data.Value.baseVal == 0)
                Times.AbsorbModifier(data.Times);
            else
                Value.AbsorbModifier(data.Value);

            description = string.Empty;
        }
    }

    public string GetElementText()
    {
        if (!description.Equals(string.Empty)) return description;
        if (!string.IsNullOrEmpty(info.txtOverride)) return info.txtOverride;

        StringBuilder sb = new StringBuilder(100);

        foreach (ModifierType mod in modifiedTypes)
            sb.Append(Helpers.ModifierToIconName[mod]);

        sb.Append(condition.GetTextCard());
        sb.Append(info.verb);
        sb.Append(" ");

        if (info.Equals(StatusEffectType.Damage))
        {
            if (Value is CardIntLinkedProperty cip)
                sb.Append(cip.GetTextForValue());
            else
                sb.Append(Helpers.ValueColorWrapper(Value.value + CharacterStats.Power, CombatSystem.instance.CalculateDisplayDamage(Value)));
        }
        else
            sb.Append(Value.GetTextForValue());

        sb.Append(String.Format(" <b>{0}</b>",info.ToString()));

        if (Times != 1) sb.Append(string.Format(" {0} times",Times.GetTextForTimes()));
        if (Target != CardTargetType.Self && Target != CardTargetType.EnemySingle) sb.Append(" " + Target.ToString());

        description = sb.ToString();
        return description;
    }

    public string GetElementToolTip() => info.toolTipCard;

    public void ForceTextRefresh()
    {
        if (card is CardCombat c)
        {
            description = string.Empty;
            c.RefreshDescriptionText(true);
        }
    }

    public void RegisterModified(ModifierType type)
    {
        if (type != ModifierType.None)
            modifiedTypes.Add(type);
    }
    public static StatusEffectCarrier operator+(StatusEffectCarrier a, StatusEffectCarrier b)
    {
        if (a == null)
            return b;
        if (b == null)
            return a;

        if(!a.info.Equals(b.info))
        {
            Debug.LogError("Two different cardEffects attempted to be spliced");
            return null;
        }

        return new StatusEffectCarrier(a.info.type, 
                                  a.Value * a.Times + b.Value * b.Times, 
                                  1, 
                                  (CardTargetType)Mathf.Max((int)a.Target, (int)b.Target)
                                  ); 
    }
}
