using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatActivityCombatCostChange : CombatActivity
{
    public override IEnumerator Execute(CombatActivitySetting data)
    {
        List<CardCombat> eligibleCards = CombatSystem.instance.Hand.Where(c => c.classType != CardClassType.Burden && c.classType != CardClassType.Enemy && c.classType != CardClassType.Torment).ToList();
        if(eligibleCards.Count != 0)
        {
            eligibleCards[UnityEngine.Random.Range(0,eligibleCards.Count)].cost.ModifyCardCost(EnergyType.Standard,data.val);
        }
        yield return null;
    }

    public override string GetDescription(CombatActivitySetting data)
    {
        return String.Format("{0} the cost of a random card by {1}.", data.val < 0 ? "Decrease" : "Increase", data.val);
    }

    public override string GetToolTip(CombatActivitySetting data)
    {
        return String.Format("A random card's cost will be {0}d for the remainder of the encounter.", data.val < 0 ? "Decrease" : "Increase", data.val);
    }
}
