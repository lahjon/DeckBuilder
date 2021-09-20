using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardActivityDrawDiscard : CardActivity
{
    public bool doneSelecting;
    public override IEnumerator Execute(string input)
    {
        int x = int.Parse(input);
        CardActivitySetting activity = new CardActivitySetting(new CardActivityData() {parameter = input, type = CardActivityType.DrawCard });
        yield return CombatSystem.instance.StartCoroutine(CardActivitySystem.instance.StartByCardActivity(activity));

        List<CardVisual> DiscardCards = new List<CardVisual>();
        if (CombatSystem.instance.Hand.Count <= 2)
        {
            DiscardCards.AddRange(CombatSystem.instance.Hand);
        }
        else
        {
            doneSelecting = false;

            CombatDeckDisplay display = CombatSystem.instance.combatDeckDisplay;
            display.OpenDeckDisplay(CardLocation.Hand, 2, null, () => { doneSelecting = true; });

            while (!doneSelecting)
                yield return null;

            DiscardCards.AddRange(display.selectedCards);
        }

        foreach(CardVisual card in DiscardCards)
            if(card is CardCombat cc)
                CombatSystem.instance.StartCoroutine(CombatSystem.instance.DiscardCard(cc));
    }

    public override string GetDescription(string input)
    {
        return "Draw " + input + ", then discard " + input;
    }

    public override string GetToolTip(string input)
    {
        return string.Empty;
    }
}
