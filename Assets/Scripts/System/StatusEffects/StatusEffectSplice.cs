using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectSplice : StatusEffect
{
    public override bool stackable { get { return false; } }

    public StatusEffectSplice() : base()
    {
        OnEndTurn = _OnEndTurn;
    }

    protected override IEnumerator _OnEndTurn()
    {
        yield return CombatSystem.instance.StartCoroutine(RecieveInput(-nrStacked));
    }

}
