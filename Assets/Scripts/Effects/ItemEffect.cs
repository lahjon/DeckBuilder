using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ItemEffect
{
    public WorldSystem world;  
    public IEffectAdder effectAdder;
    public ItemEffectStruct itemEffectStruct;

    public virtual void ApplyEffect()
    {
        effectAdder.NotifyUsed();
    }
 
    public virtual void Register()
    {
        effectAdder.NotifyRegister();
    }

    public virtual void DeRegister()
    {
        effectAdder.NotifyDeregister();
    }

    public virtual IEnumerator RunEffectEnumerator()
    {
        yield return null;
    }


    public static ItemEffect Factory(ItemEffectStruct itemEffectStruct, IEffectAdder itemEffectAdder)
    {
        string effectName = itemEffectStruct.type != ItemEffectType.Custom ? itemEffectStruct.type.ToString() : itemEffectStruct.parameter;

        if (Helpers.InstanceObject<ItemEffect>(string.Format("ItemEffect{0}", effectName)) is ItemEffect itemEffect)
        {
            itemEffect.world = WorldSystem.instance;
            itemEffect.effectAdder = itemEffectAdder;
            itemEffect.itemEffectStruct = itemEffectStruct;
            return itemEffect;
        }
        return null;
    }
}


[System.Serializable]
public struct ItemEffectStruct
{
    public ItemEffectType type;
    public string parameter;
    public bool addImmediately;
    public int value;

    public ItemEffectStruct(ItemEffectType aType, string aParm, int aValue, bool anInstant)
    {
        type = aType;
        parameter = aParm;
        value = aValue;
        addImmediately = anInstant;
    }
    public bool ValidateStruct()
    {
        bool result = false;
        try
        {
            switch (type)
            {
                case ItemEffectType.None:
                    break;
                case ItemEffectType.Custom:
                    break;
                case ItemEffectType.Heal:
                    break;
                case ItemEffectType.AddStat:
                    parameter.ToEnum<StatType>();
                    break;
                case ItemEffectType.AddCombatEffect:
                    parameter.ToEnum<StatusEffectType>();
                    break;

                default:
                    break;
            }
            result = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception Message: " + ex.Message);
        }
        return result;
    }
}

public enum ItemEffectType
{
    None = 0,
    Custom,
    AddStat,                // parm = StatType | value = value
    AddCombatEffect,
    AddCombatActivity,
    TakeDamage,
    Heal,
    AddCardDeck,
    AddCardDeckCombat
}

public interface IEffectAdder
{
    public void NotifyUsed();
    public string GetName();

    public void NotifyRegister();
    public void NotifyDeregister();
}
public struct IEffectAdderStruct : IEffectAdder
{
    string name;
    int value;
    public IEffectAdderStruct(string aName, int aValue)
    {
        name = aName;
        value = aValue;
    }
    public string GetName() => name;
    public int GetValue() => value;

    public void NotifyDeregister()
    {
    }

    public void NotifyRegister()
    {
    }

    public void NotifyUsed()
    {

    }
}
