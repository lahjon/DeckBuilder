using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectCharge : CardEffect
{
    public override bool isBuff { get { return true; } }
    public override bool stackable { get { return true; } }

    public CardEffectCharge() : base()
    {
        OnEndTurn = null;
    }



}
