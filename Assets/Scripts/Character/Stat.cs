using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Stat
{
    public Stat(int aValue, StatType aType, bool aVisible)
    {
        value = aValue;
        type = aType;
        visible = aVisible;
    }
    [SerializeField] List<IEffectAdder> _statModifers = new List<IEffectAdder>();
    public int value;
    public bool visible;
    public StatType type;
    public  List<IEffectAdder> StatModifers
    {
        get => _statModifers;
    }

    public void AddStatModifier(IEffectAdder statModifer)
    {
        _statModifers.Add(statModifer);
        EventManager.StatModified();
    }

    public void RemoveStatModifier(IEffectAdder statModifer)
    {
        _statModifers.Remove(statModifer);
        EventManager.StatModified();
    }
}