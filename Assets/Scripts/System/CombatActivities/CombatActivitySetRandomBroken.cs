
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatActivitySetRandomBroken : CombatActivity
{
    public override IEnumerator Execute(CombatActivitySetting data)
    {
        List<CardCombat> eligibleCards = CombatSystem.instance.Hand.Where(c => c.classType != CardClassType.Burden && c.classType != CardClassType.Enemy && c.classType != CardClassType.Torment).ToList();
        int index = Random.Range(0, eligibleCards.Count);
        eligibleCards[index].isBroken = true;
        yield return null;
    }

    public override string GetDescription(CombatActivitySetting data)
    {
        return "Break a random card";
    }

    public override string GetToolTip(CombatActivitySetting data)
    {
        return "A random card will be broken";
    }
}
