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
    [SerializeField] List<StatModifer> _statModifers = new List<StatModifer>();
    public int value;
    public bool visible;
    public StatType type;
    public  List<StatModifer> StatModifers
    {
        get => _statModifers;
    }

    public void AddStatModifier(StatModifer statModifer)
    {
        _statModifers.Add(statModifer);
        EventManager.StatModified();
    }

    public void RemoveStatModifier(StatModifer statModifer)
    {
        _statModifers.Remove(statModifer);
        EventManager.StatModified();
    }

    public void RemoveStatModifier(IEffectAdder effectAdder)
    {
        RemoveStatModifier(_statModifers.FirstOrDefault(x => x.effectAdder == effectAdder));
    }
}