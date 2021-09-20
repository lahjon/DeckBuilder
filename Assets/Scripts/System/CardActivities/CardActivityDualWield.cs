using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardActivityDualWield : CardActivity
{
    public bool doneSelecting;
    public override IEnumerator Execute(string input)
    {
        int x = int.Parse(input);
        doneSelecting = false;

        List<Card> selectableCards = new List<Card>(CombatSystem.instance.Hand.Where(x => x.cardType == CardType.Attack).ToList());
        
        if(selectableCards.Count == 0)
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning("No attack card to choose!");
        else
        {
            CombatDeckDisplay display = CombatSystem.instance.combatDeckDisplay;
            display.OpenDeckDisplay(CardLocation.Hand, 1, selectableCards, () => { doneSelecting = true; });

            while (!doneSelecting)
                yield return null;

            string cmd = "";
            for (int i = 0; i < x; i++)
                cmd += display.selectedCards[0].cardData.name + ",Hand" + (i == x - 1 ? "" : ";");

            CardActivitySetting activity = new CardActivitySetting(new CardActivityData() {parameter = cmd, type = CardActivityType.AddCardToCombat });
            yield return CombatSystem.instance.StartCoroutine(CardActivitySystem.instance.StartByCardActivity(activity));

            CardFunctionalityData modData = new CardFunctionalityData();
            modData.singleFieldProperties.Add(new CardSingleFieldPropertyTypeWrapper(CardSingleFieldPropertyType.Exhaust, true));

            for(int i = 0; i < x; i++)
            {
                CardCombat card = CombatSystem.instance.Hand[CombatSystem.instance.Hand.Count - 1 - i];
                card.cost.ModifyCardCost(-1);
                card.AddModifierToCard(modData);
            }
        }
    }

    public override string GetDescription(string input)
    {
        return "Choose an attack card. Add " + input + " copies of that card to your hand. They cost 1 less, and have exhaust";
    }

    public override string GetToolTip(string input)
    {
        return string.Empty;
    }
}
