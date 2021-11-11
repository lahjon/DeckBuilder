using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ItemEffectManager : Manager
{
    protected override void Awake()
    {
        base.Awake();
        world.itemEffectManager = this;
    }
    public static ItemEffect CreateItemEffect(ItemEffectStruct itemEffectStruct, bool triggerInstant = true)
    {
        if (InstanceObject(string.Format("ItemEffect{0}", itemEffectStruct.type.ToString())) is ItemEffect itemEffect)
        {
            itemEffect.world = world;
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
    public bool instant;
    public int value;
    public ItemEffectStruct(ItemEffectType aType, string aParm, int aValue, bool anInstant)
    {
        type = aType;
        parameter = aParm;
        value = aValue;
        instant = anInstant;
    }
}
