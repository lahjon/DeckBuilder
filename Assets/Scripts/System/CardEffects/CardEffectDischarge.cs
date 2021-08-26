using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectDischarge : CardEffect
{
    public override bool isBuff { get { return false; } }
    public override bool stackable { get { return false; } }

    public CardEffectDischarge() : base()
    {
        OnEndTurn = null;
    }



}
