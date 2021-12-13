using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatActivityModifyEnergy : CombatActivity
{
    public override IEnumerator Execute(CombatActivitySetting data)
    {
        CombatSystem.instance.ModifyEnergy(data.strParameter.ToEnum<EnergyType>(), data.val);
        yield return new WaitForSeconds(0.2f); // byt mot energy effekt
    }

    public override string GetDescription(CombatActivitySetting data)
    {
        return (data.val < 0 ? "Loose " : "Gain ") + data.val + " energy";
    }

    public override string GetToolTip(CombatActivitySetting data)
    {
        return string.Empty;
    }
}
