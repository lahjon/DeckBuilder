using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class ItemEffect
{
    public WorldSystem world;  
    public IEffectAdder effectAdder;
    public ItemEffectStruct itemEffectStruct;
    public virtual void AddItemEffect()
    {
        world.itemEffectManager.allActiveItemEffects.Add(this);
        effectAdder.NotifyUsed();
    }
    public virtual void RemoveItemEffect()
    {
        world.itemEffectManager.allActiveItemEffects.Remove(this);
    }
}