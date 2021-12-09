using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatActivityModifyLife : CombatActivity
{
    public override IEnumerator Execute(CombatActivitySetting data)
    {
        if (data.val >= 0)
            CombatSystem.instance.ActiveActor.HealLife(data.val);
        else
            CombatSystem.instance.ActiveActor.LooseLife(-data.val);

        yield return null;
    }

    public override string GetDescription(CombatActivitySetting data)
    {
        return string.Format("{0} {1} life",data.val >= 0 ? "Heal" : "Loose", data.val >= 0 ? data.val : -data.val);
    }

    public override string GetToolTip(CombatActivitySetting data)
    {
        return string.Empty;
    }
}
