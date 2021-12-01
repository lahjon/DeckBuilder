using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardActivityEN_AddCardModifier : CardActivity
{
    readonly string toolTip = "<b>Cursed</b>\nThis card has adverse effects added to it.";
    public override IEnumerator Execute(CardActivitySetting data)
    {
        CardCombat card;
        CombatActor Hero = CombatSystem.instance.Hero;
        if (Hero.deck.Count == 0 && Hero.discard.Count == 0)
            card = null;
        else
        {
            List<Card> cardPile = Hero.deck.Count != 0 ? Hero.deck : Hero.discard;
            card = (CardCombat)cardPile[UnityEngine.Random.Range(0, cardPile.Count)];

            CardFunctionalityData addingComponent = DatabaseSystem.instance.cardModifiers.Where(x => x.id == data.strParameter).FirstOrDefault();
            if (addingComponent == null) Debug.Log("No cardmodder with id " + data.strParameter);
            else
            {
                for(int i = 0; i < data.val-1;i++)
                    card.AddModifierToCard(addingComponent, ModifierType.Cursed, true);
                card.AddModifierToCard(addingComponent, ModifierType.Cursed);
            }
            card.SetManualToolTip(toolTip);
            yield return null;
        }
    }

    public override string GetDescription(CardActivitySetting data) => string.Empty;

    public override string GetToolTip(CardActivitySetting data) => string.Empty;
}
