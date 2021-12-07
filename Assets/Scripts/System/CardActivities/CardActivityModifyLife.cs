using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActivityModifyLife : CardActivity
{
    public override IEnumerator Execute(CardActivitySetting data)
    {
        if (data.val >= 0)
            CombatSystem.instance.ActiveActor.HealLife(data.val);
        else
            CombatSystem.instance.ActiveActor.LooseLife(-data.val);

        yield return null;
    }

    public override string GetDescription(CardActivitySetting data)
    {
        return string.Format("{0} {1} life",data.val >= 0 ? "Heal" : "Loose", data.val >= 0 ? data.val : -data.val);
    }

    public override string GetToolTip(CardActivitySetting data)
    {
        return string.Empty;
    }
}
