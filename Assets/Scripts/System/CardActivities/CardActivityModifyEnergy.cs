using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardActivityModifyEnergy : CardActivity
{
    public override IEnumerator Execute(CardActivitySetting data)
    {
        CombatSystem.instance.cEnergy += data.val;
        yield return new WaitForSeconds(0.2f); // byt mot energy effekt
    }

    public override string GetDescription(CardActivitySetting data)
    {
        return (data.val < 0 ? "Loose " : "Gain ") + data.val + " energy";
    }

    public override string GetToolTip(CardActivitySetting data)
    {
        return string.Empty;
    }
}
