using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardActivityDrawDiscard : CardActivity
{
    public bool doneSelecting;
    public override IEnumerator Execute(CardActivitySetting data)
    {
        CardActivitySetting activity = new CardActivitySetting(new CardActivityData() {val = data.val, type = CardActivityType.DrawCard });
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
            display.OpenDeckDisplay(CardLocation.Hand, data.val, null, () => { doneSelecting = true; });

            while (!doneSelecting)
                yield return null;

            DiscardCards.AddRange(display.selectedCards);
        }

        foreach(CardVisual card in DiscardCards)
            if(card is CardCombat cc)
                CombatSystem.instance.StartCoroutine(CombatSystem.instance.DiscardCard(cc));
    }

    public override string GetDescription(CardActivitySetting data)
    {
        return "Draw " + data.val + ", then discard " + data.val;
    }

    public override string GetToolTip(CardActivitySetting data)
    {
        return string.Empty;
    }
}
