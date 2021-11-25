using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour, IEffectAdder
{
    public ItemEffect itemEffect;
    public string itemName;
    public virtual void AddEffect()
    {
        //WorldSystem.instance.itemEffectManager.RemoveItemEffectAdder(this);
    }
    public virtual void RemoveEffect()
    {
        //WorldSystem.instance.itemEffectManager.RemoveItemEffectAdder(this);
    }


    public abstract void NotifyUsed();

    protected void OnDestroy()
    {
        RemoveEffect();
    }

    //public int GetValue() => itemEffect.itemEffectStruct.value;
    public int GetValue()
    {
        if (itemEffect != null)
            return itemEffect.itemEffectStruct.value;
        else
            return 0;
    }

    public string GetName() => itemName;
}