using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardEffect
{
    public EffectType Type;
    public int Value;
    public int Times;
    public CardTargetType Target = CardTargetType.EnemySingle;
}
