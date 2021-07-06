using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActivityHeal : CardActivity
{
    public override IEnumerator Execute(string input)
    {
        int x = Int32.Parse(input);
        CombatSystem.instance.ActiveActor.HealLife(x);
        yield return null;
    }

    public override string GetDescription(string input)
    {
        return "Heal " + input + " life";
    }

    public override string GetToolTip(string input)
    {
        return $"<b>Heal</b> This will restore some life lost";
    }
}
