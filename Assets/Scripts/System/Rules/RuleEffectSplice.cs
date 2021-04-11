using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleEffectSplice : RuleEffect
{
    public override bool isBuff { get { return true; } }

    public override void OnEndTurn()
    {
        nrStacked = 0;
    }

}
