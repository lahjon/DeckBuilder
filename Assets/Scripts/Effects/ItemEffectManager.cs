using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ItemEffectManager : Manager
{
    public HashSet<ItemEffect> allActiveItemEffects = new HashSet<ItemEffect>();
    //public HashSet<ItemEffect> allItemStructs = new HashSet<ItemEffect>();
    protected override void Awake()
    {
        base.Awake();
        world.itemEffectManager = this;
    }
    public ItemEffect CreateItemEffect(ItemEffectStruct itemEffectStruct, IEffectAdder itemEffectAdder, bool triggerInstant = true)
    {
        string effectName = itemEffectStruct.type.ToString();
        if (itemEffectStruct.type == ItemEffectType.Custom)
            effectName = itemEffectStruct.parameter;

        if (InstanceObject(string.Format("ItemEffect{0}", effectName)) is ItemEffect itemEffect)
        {
            itemEffect.world = world;
            itemEffect.effectAdder = itemEffectAdder;
            itemEffect.itemEffectStruct = itemEffectStruct;
            if (triggerInstant) itemEffect.AddItemEffect();
            return itemEffect;
        }
        return null;
    }

    static ItemEffect InstanceObject(string aName)
    {
        if (Type.GetType(aName) is Type type && (ItemEffect)Activator.CreateInstance(type, world) is ItemEffect itemEffect)
            return itemEffect;
        else
            return null;
    }
}

[System.Serializable]
public struct ItemEffectStruct
{
    public ItemEffectType type;
    public string parameter;
    public bool addOnStart;
    public int value;
    public ItemEffectStruct(ItemEffectType aType, string aParm, int aValue, bool anInstant)
    {
        type = aType;
        parameter = aParm;
        value = aValue;
        addOnStart = anInstant;
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
                    parameter.ToEnum<EffectType>();
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
    TakeDamage,
    Heal
}

public interface IEffectAdder
{
    public void NotifyUsed();
    public int GetValue();
    public string GetName();
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
    public void NotifyUsed()
    {

    }
}
