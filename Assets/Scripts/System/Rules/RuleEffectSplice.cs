using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleEffectSplice : RuleEffect
{
    public override bool isBuff { get { return true; } }
    public override bool triggerRecalcDamage { get { return false; } }

    public override void OnEndTurnBehaviour()
    {
        nrStacked = 0;
    }

}
