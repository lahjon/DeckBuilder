using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardActivityEN_AddCardModifier : CardActivity
{
    readonly string toolTip = "<b><sprite name=\"Cursed\"> Cursed</b>\nThis card has adverse effects added to it.";
    public override IEnumerator Execute(CardActivitySetting data)
    {
        CombatActor Hero = CombatSystem.instance.Hero;
        if (!(Hero.deck.Count == 0 && Hero.discard.Count == 0))
        {
            int nrCards;
            string modId;

            if (data.strParameter.Contains(":"))
            {
                string[] input = data.strParameter.Split(':');
                nrCards = input[0].ToInt();
                modId = input[1];
            }
            else
            {
                nrCards = 1;
                modId = data.strParameter;
            }

            List<Card> cardPile = new List<Card>(Hero.deck.Count != 0 ? Hero.deck : Hero.discard);
            List<CardCombat> chosenCards = new List<CardCombat>();

            for (int i = 0; i < nrCards; i++)
            {
                int index = UnityEngine.Random.Range(0, cardPile.Count);
                chosenCards.Add((CardCombat)cardPile[index]);
                cardPile.RemoveAt(index);
                if (cardPile.Count == 0)
                    cardPile.AddRange(Hero.deck.Count != 0 ? Hero.deck : Hero.discard);
            }

            foreach (CardCombat card in chosenCards)
            {
                CardFunctionalityData addingComponent = DatabaseSystem.instance.cardModifiers.Where(x => x.id == data.strParameter).FirstOrDefault();
                if (addingComponent == null) Debug.Log("No cardmodder with id " + data.strParameter);
                else
                {
                    for (int i = 0; i < data.val - 1; i++)
                        card.AddModifierToCard(addingComponent, ModifierType.Blessed, true);
                    card.AddModifierToCard(addingComponent, ModifierType.Blessed);
                }
                card.SetManualToolTip(toolTip);
            }

            yield return null;
        }
    }

    public override string GetDescription(CardActivitySetting data) => string.Empty;

    public override string GetToolTip(CardActivitySetting data) => string.Empty;
}
