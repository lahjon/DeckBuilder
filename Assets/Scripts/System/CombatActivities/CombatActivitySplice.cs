using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatActivitySplice : CombatActivity
{
    public override IEnumerator Execute(CombatActivitySetting data)
    {
        StatusEffectCarrier cardEffect;
        CombatActor hero = CombatSystem.instance.Hero;

        if (!hero.effectTypeToRule.ContainsKey(StatusEffectType.Splice)) {
            cardEffect = new StatusEffectCarrier(StatusEffectType.Splice, data.val);
            yield return CombatSystem.instance.StartCoroutine(hero.RecieveEffectNonDamageNonBlock(cardEffect));
        }
        else
        {
            CardCombat discardedCard = null;
            for(int i = 0; i < hero.discard.Count; i++)
            {
                if(hero.discard[i].activitiesOnPlay.Count(x => x.type == CombatActivityType.Splice) != 0)
                {
                    discardedCard = (CardCombat)hero.discard[i];
                    break;
                }
            }
            
            if (discardedCard != null)
            {
                cardEffect = new StatusEffectCarrier(StatusEffectType.Splice, -1);
                yield return CombatSystem.instance.StartCoroutine(CombatSystem.instance.ActiveActor.RecieveEffectNonDamageNonBlock(cardEffect));
                CardCombat splicedCard = CardCombat.Combine((CardCombat)CombatSystem.instance.InProcessCard, discardedCard);
                CombatSystem.instance.InProcessCard.RegisterSingleField(new CardSingleFieldPropertyTypeWrapper() {prop = CardSingleFieldPropertyType.Exhaust, val = true});

                hero.discard.Remove(discardedCard);
                UnityEngine.Object.Destroy(discardedCard.gameObject);

                CombatSystem.instance.UpdateDeckTexts();
                CombatSystem.instance.cardPresenter.DisplayCards(new List<CardCombat>() { splicedCard }, new List<CardLocation>() { CardLocation.Discard });
            }
        }
    }

    public override string GetDescription(CombatActivitySetting data)
    {
        return "<b>Splice</b>" + data.val;
    }

    public override string GetToolTip(CombatActivitySetting data)
    {
        return $"If you play this card while you have <b>Splice</b>, add the contents of the top card with <b>Splice</b> in your discard pile to this card. " +
            $"\nOtherwise, recieve the <b>Splice</b> status effect. " +
            $"\nThis card can merge " + data.val  + " more time" + (data.val  == 1 ? "" : "s");
    }
}
