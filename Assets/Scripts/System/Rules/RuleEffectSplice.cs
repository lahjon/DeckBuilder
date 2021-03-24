using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleEffectSplice : RuleEffect
{
    public override void OnEndTurnBehaviour()
    {
        nrStacked = 0;
    }

}
