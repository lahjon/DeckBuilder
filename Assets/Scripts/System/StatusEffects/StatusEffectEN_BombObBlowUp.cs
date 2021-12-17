using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatusEffectEN_BombObBlowUp : StatusEffect, ICombatEffect
{
    public override bool triggerRecalcDamageSelf { get { return false; } }

    public StatusEffectEN_BombObBlowUp() : base()
    {
        OnEndTurn = null;
        OnNewTurn = null;
    }

    public override void AddFunctionToRules() => EventManager.OnActorLostLifeEvent += OnActorLostLife;

    public override void RemoveFunctionFromRules() => EventManager.OnActorLostLifeEvent += OnActorLostLife;

    public void OnActorLostLife(CombatActor actor, int amount)
    {
        if (actor == this.actor && 100 * actor.hitPoints / actor.maxHitPoints < nrStacked) CombatSystem.instance.QueueEffect(this);
    }

    public IEnumerator RunEffectEnumerator()
    {
        actor.healthEffectsUI.StartNotificationEffect("Triggered!");
        actor.deck.Clear(); 
        actor.discard.Clear();

        CardData data = DatabaseSystem.instance.cards.FirstOrDefault(c => c.id == "93");

        CombatActorEnemy thisGuy = ((CombatActorEnemy)actor);
        thisGuy.AddEnemyCardFromData(data);
        thisGuy.AddEnemyCardFromData(data);
        yield return new WaitForSeconds(0.3f);
        thisGuy.DrawCard();
        yield return new WaitForSeconds(0.3f);

        RemoveFunctionFromRules();
    }



}
