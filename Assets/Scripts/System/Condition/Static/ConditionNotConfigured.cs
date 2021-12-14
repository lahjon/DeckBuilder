    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ConditionNotConfigured : Condition
{
    public override void Subscribe(){}
    public override void Unsubscribe() { }
    public override bool ConditionEvaluator()
    {
        return true;
    }
}


