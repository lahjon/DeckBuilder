using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectLoaded : CardEffect
{
    public override bool isBuff { get { return true; } }
    public override bool triggerRecalcDamageSelf { get { return true; } }

    public CardEffectLoaded() : base()
    {
        OnEndTurn = null;
        OnNewTurn = null;
    }

    public override void AddFunctionToRules()
    {
        actor.dealAttackMult.Add(Doubler);
        EventManager.OnCardPlayEvent += CardPlayed;
    }

    public override void RemoveFunctionFromRules()
    {
        actor.dealAttackMult.Remove(Doubler);
        EventManager.OnCardPlayEvent -= CardPlayed;
    }


    public float Doubler() => 2f;

    public void CardPlayed(Card card)
    {
        if (card.owner == actor && card.cardType == CardType.Attack)
            CombatSystem.instance.StartCoroutine(RecieveInput(-1));
    }

}
