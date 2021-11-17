using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class ItemEffect
{
    public WorldSystem world;
    public string sourceName; 
    public ItemEffectStruct itemEffectStruct;
    public virtual void AddItemEffect()
    {
        world.itemEffectManager.itemEffects.Add(this);
    }
    public virtual void RemoveItemEffect()
    {
        world.itemEffectManager.itemEffects.Remove(this);
    }
}