using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatActivityDualWield : CombatActivity
{
    public bool doneSelecting;
    public override IEnumerator Execute(CombatActivitySetting data)
    {
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
            for (int i = 0; i < data.val; i++)
                cmd += display.selectedCards[0].cardData.name + ",Hand" + (i == data.val - 1 ? "" : ";");

            CombatActivitySetting activity = new CombatActivitySetting(new CardActivityData() {strParameter = cmd, type = CombatActivityType.AddCardToCombat });
            yield return CombatSystem.instance.StartCoroutine(CombatActivitySystem.instance.StartByCardActivity(activity));

            List<CardFunctionalityData> modData = new List<CardFunctionalityData>(display.selectedCards[0].cardModifiers);
            CardFunctionalityData exhauster = DatabaseSystem.instance.cardModifiers.Where(x => x.id == "-1_2").FirstOrDefault();
            modData.Add(exhauster);

            for(int i = 0; i < data.val; i++)
            {
                CardCombat card = CombatSystem.instance.Hand[CombatSystem.instance.Hand.Count - 1 - i];
                card.cost.ModifyCardCost(EnergyType.Standard,-1);
                for(int j = 0; j< modData.Count; j++)
                    card.AddModifierToCard(modData[j]);
            }
        }
    }

    public override string GetDescription(CombatActivitySetting data)
    {
        return "Choose an attack card. Add " + data.val + " copies of that card to your hand. They cost 1 less, and have exhaust";
    }

    public override string GetToolTip(CombatActivitySetting data)
    {
        return string.Empty;
    }
}
