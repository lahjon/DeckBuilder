using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectDummmy : CardEffect
{
    public override bool triggerRecalcDamageSelf { get { return false; } } //manual call of recalc in code

    public CardEffectDummmy() : base()
    {
        OnEndTurn = null;
    }
}
