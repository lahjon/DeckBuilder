using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatModifer
{
    public IEffectAdder effectAdder;
    // public StatModifer(int aValue, IEffectAdder aEffectAdder, StatType aStatType)
    // {
    //     value = aValue;
    //     effectAdder = aEffectAdder;
    //     statType = aStatType;
    // }
    public StatModifer(int aValue, IEffectAdder aEffectAdder)
    {
        value = aValue;
        effectAdder = aEffectAdder;
    }
    //public StatType statType;
    public int value;

}
