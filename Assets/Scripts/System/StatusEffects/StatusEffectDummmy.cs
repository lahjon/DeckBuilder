using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectDummmy : StatusEffect
{
    public override bool triggerRecalcDamageSelf { get { return false; } } //manual call of recalc in code

    public StatusEffectDummmy() : base()
    {
        OnEndTurn = null;
    }
}
