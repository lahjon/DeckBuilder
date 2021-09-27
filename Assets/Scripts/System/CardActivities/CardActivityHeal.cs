using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActivityHeal : CardActivity
{
    public override IEnumerator Execute(CardActivitySetting data)
    {
        CombatSystem.instance.ActiveActor.HealLife(data.val);
        yield return null;
    }

    public override string GetDescription(CardActivitySetting data)
    {
        return "Heal " + data.val + " life";
    }

    public override string GetToolTip(CardActivitySetting data)
    {
        return string.Empty;
    }
}
