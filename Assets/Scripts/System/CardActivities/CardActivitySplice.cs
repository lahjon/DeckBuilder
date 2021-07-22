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
                CombatSystem.instance.InProcessCard.exhaust = true;

                hero.discard.Remove(discardedCard);
                UnityEngine.Object.Destroy(discardedCard.gameObject);

                CombatSystem.instance.UpdateDeckTexts();
                CombatSystem.instance.cardPresenter.DisplayCards(new List<CardCombat>() { splicedCard }, new List<CardLocation>() { CardLocation.Discard });
            }
        }
    }

    public override string GetDescription(string input)
    {
        return "<b>Splice</b>" + input;
    }

    public override string GetToolTip(string input)
    {
        return $"If you play this card while you have <b>Splice</b>, add the contents of the top card with <b>Splice</b> in your discard pile to this card. " +
            $"\nOtherwise, recieve the <b>Splice</b> status effect. " +
            $"\nThis card can merge " + input + " more time" + (input.Equals("1") ? "" : "s");
    }
}
