using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class ItemEffect
{
    public WorldSystem world;
    public ItemEffectStruct itemEffectStruct;
    public abstract void AddItemEffect();
    public abstract void RemoveItemEffect();
}