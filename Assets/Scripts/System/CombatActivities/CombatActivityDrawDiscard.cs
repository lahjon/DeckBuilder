using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatActivityDrawDiscard : CombatActivity
{
    public bool doneSelecting;
    public override IEnumerator Execute(CombatActivitySetting data)
    {
        CombatActivitySetting activity = new CombatActivitySetting(new CardActivityData() {val = data.val, type = CombatActivityType.DrawCard });
        yield return CombatSystem.instance.StartCoroutine(CombatActivitySystem.instance.StartByCardActivity(activity));

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

    public override string GetDescription(CombatActivitySetting data)
    {
        return "Draw " + data.val + ", then discard " + data.val;
    }

    public override string GetToolTip(CombatActivitySetting data)
    {
        return string.Empty;
    }
}
