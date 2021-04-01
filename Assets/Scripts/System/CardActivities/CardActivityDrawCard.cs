using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActivityDrawCard : CardActivity
{

    public override IEnumerator Execute(string input)
    {
        int x = Int32.Parse(input);
        yield return ActivitySystem.StartCoroutine(combatController.DrawCards(x));
    }

    public override string GetDescription(string input)
    {
        return "Draw" + input + " Cards.";
    }

    public override string GetToolTip(string input)
    {
        return $"When you play this card, draw " + (input == "1" ? "a card." : input + " cards.");
    }
}
