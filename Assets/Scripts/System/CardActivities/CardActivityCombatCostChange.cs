using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardActivityCombatCostChange : CardActivity
{
    public override IEnumerator Execute(string input)
    {
        List<CardCombat> eligibleCards = CombatSystem.instance.Hand.Where(c => c.classType != CardClassType.Burden && c.classType != CardClassType.Enemy && c.classType != CardClassType.Torment).ToList();
        if(eligibleCards.Count != 0)
        {
            eligibleCards[UnityEngine.Random.Range(0,eligibleCards.Count)].cost.value += 1;
        }
        yield return null;
    }

    public override string GetDescription(string input)
    {
        return String.Format("{0} the cost of a random card by {1}.", input.Substring(0,1).Equals("-") ? "Decrease" : "Increase", input);
    }

    public override string GetToolTip(string input)
    {
        return String.Format("A random card's cost will be {0}d for the remainder of the encounter.", input.Substring(0, 1).Equals("-") ? "Decrease" : "Increase", input);
    }
}
