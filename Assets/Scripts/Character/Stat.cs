using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    public List<IEffectAdder> allEffects = new List<IEffectAdder>();
    public Stat(int aValue, StatType aType)
    {
        value = aValue;
        type = aType;
    }
    public int value;
    public StatType type;
}