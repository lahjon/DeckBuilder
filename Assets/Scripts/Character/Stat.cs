using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Stat
{
    public static implicit operator int(Stat s) => s.value;
    public Stat() { }
    
    public Stat(StatType aType, bool aVisible)
    {
        type = aType;
        visible = aVisible;
    }
    [SerializeField] public List<ItemEffectAddStat> statModifers = new List<ItemEffectAddStat>();
    
    public int value;
    public bool visible;
    public StatType type;

    public virtual void AddStatModifier(ItemEffectAddStat statModifer)
    {
        statModifers.Add(statModifer);
        value += statModifer.GetValue();
        EventManager.StatModified();
    }

    public virtual void RemoveStatModifier(ItemEffectAddStat statModifer)
    {
        statModifers.Remove(statModifer);
        value -= statModifer.GetValue();
        EventManager.StatModified();
    }

    public void RemoveModifierFromOwner(IEffectAdder owner)
    {
        ItemEffectAddStat modder = statModifers.Where(x => x.effectAdder == owner).FirstOrDefault();
        if (modder != null) RemoveStatModifier(modder);
    }

}