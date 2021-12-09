using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatActivityDrawCard : CombatActivity
{

    public override IEnumerator Execute(CombatActivitySetting data)
    {
        yield return ActivitySystem.StartCoroutine(CombatSystem.instance.DrawCards(data.val));
    }

    public override string GetDescription(CombatActivitySetting data)
    {
        return "Draw " + (data.val == 1 ? "a card." : data.val + " cards.");
    }

    public override string GetToolTip(CombatActivitySetting data)
    {
        return string.Empty;
    }
}
