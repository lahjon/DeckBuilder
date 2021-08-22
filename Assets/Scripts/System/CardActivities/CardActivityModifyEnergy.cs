using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardActivityModifyEnergy : CardActivity
{
    public override IEnumerator Execute(string input)
    {
        CombatSystem.instance.cEnergy += int.Parse(input);
        yield return new WaitForSeconds(0.2f); // byt mot energy effekt
    }

    public override string GetDescription(string input)
    {
        return (input.Substring(0,1).Equals("-") ? "Loose " : "Gain ") + input + " energy";
    }

    public override string GetToolTip(string input)
    {
        return string.Empty;
    }
}
