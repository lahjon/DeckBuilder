using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectWound : StatusEffect
{
    public StatusEffectWound() : base()
    {
        OnEndTurn = null;
    }

    public override void AddFunctionToRules() => actor.takeAttackLinear.Add(TakeAttackLinear);
    public override void RemoveFunctionFromRules() => actor.takeAttackLinear.Remove(TakeAttackLinear);
    public int TakeAttackLinear() => nrStacked;
    public override void RespondStackUpdate(int update) => CombatSystem.instance.EnemiesInScene.ForEach(e => e.RecalcDamage());
}
