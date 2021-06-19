using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectSplice : CardEffect
{
    public override bool isBuff { get { return true; } }

    public override void OnEndTurn()
    {
        nrStacked = 0;
    }

}
