using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardActivitySplice : CardActivity
{
    CardEffect cardEffect = new CardEffect() { Type = EffectType.Splice, Times = 1};
    public override IEnumerator Execute(string input)
    {
        if (!combatController.Hero.healthEffects.effectTypeToRule.ContainsKey(EffectType.Splice)) {
            cardEffect.Value = Int32.Parse(input);
            combatController.Hero.healthEffects.RecieveEffectNonDamageNonBlock(cardEffect);
        }
        else
        {
            CardCombat discardedCard = null;
            for(int i = combatController.Discard.Count -1; i >= 0; i--)
            {
                if(combatController.Discard[i].activities.Count(x => x.type == CardActivityType.Splice) != 0)
                {
                    discardedCard = combatController.Discard[i];
                    break;
                }
            }
            
            if (discardedCard != null)
            {
                cardEffect.Value = -1;
                combatController.ActiveActor.healthEffects.RecieveEffectNonDamageNonBlock(cardEffect);
                CardCombat splicedCard = CardCombat.CreateCardCombined((CardCombat)combatController.CardInProcess.card, discardedCard);
                combatController.Deck.Add(splicedCard);
                combatController.CardInProcess.card.exhaust = true;
                combatController.Discard.Remove(discardedCard);
                CombatController.Destroy(discardedCard.gameObject);
                combatController.UpdateDeckTexts();

                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Created spliced card!");
            }
        }
        yield return null;
    }

    public override string GetDescription(string input)
    {
        return "<b>Splice</b>" + input;
    }

    public override string GetToolTip(string input)
    {
        return $"If you play this card while you have at leat one <b>Splice</b> status, combine this card with the top card having <b>Splice</b> in your discard pile. Otherwise, recieve the <b>Splice</b> status effect.";
    }
}
