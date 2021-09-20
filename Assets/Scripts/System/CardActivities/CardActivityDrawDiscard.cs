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
        doneSelecting = false;

        CombatDeckDisplay display = CombatSystem.instance.combatDeckDisplay;
        display.OpenDeckDisplay(CardLocation.Hand, 2, () => { doneSelecting = true; });

        while (!doneSelecting)
            yield return null;

        foreach(CardVisual card in display.selectedCards)
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
