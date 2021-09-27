using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActivityDrawCard : CardActivity
{

    public override IEnumerator Execute(CardActivitySetting data)
    {
        yield return ActivitySystem.StartCoroutine(CombatSystem.instance.DrawCards(data.val));
    }

    public override string GetDescription(CardActivitySetting data)
    {
        return "Draw " + (data.val == 1 ? "a card." : data.val + " cards.");
    }

    public override string GetToolTip(CardActivitySetting data)
    {
        return string.Empty;
    }
}
