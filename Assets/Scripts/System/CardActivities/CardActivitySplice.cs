using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardActivitySplice : CardActivity
{
    CardEffectInfo cardEffect = new CardEffectInfo() { Type = EffectType.Splice, Times = 1};
    public override IEnumerator Execute(string input)
    {
        CombatActor hero = CombatSystem.instance.Hero;

        if (!hero.effectTypeToRule.ContainsKey(EffectType.Splice)) {
            cardEffect.Value = Int32.Parse(input);
            yield return CombatSystem.instance.StartCoroutine(hero.RecieveEffectNonDamageNonBlock(cardEffect));
        }
        else
        {
            CardCombat discardedCard = null;
            for(int i = 0; i < hero.discard.Count; i++)
            {
                if(hero.discard[i].activitiesOnPlay.Count(x => x.type == CardActivityType.Splice) != 0)
                {
                    discardedCard = (CardCombat)hero.discard[i];
                    break;
                }
            }
            
            if (discardedCard != null)
            {
                cardEffect.Value = -1;
                yield return CombatSystem.instance.StartCoroutine(CombatSystem.instance.ActiveActor.RecieveEffectNonDamageNonBlock(cardEffect));
                CardCombat splicedCard = CardCombat.CreateCardCombined((CardCombat)CombatSystem.instance.InProcessCard, discardedCard);
                hero.deck.Add(splicedCard);
                CombatSystem.instance.InProcessCard.exhaust = true;
                hero.discard.Remove(discardedCard);
                CombatSystem.Destroy(discardedCard.gameObject);
                CombatSystem.instance.UpdateDeckTexts();

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
