
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatActivityEmptySavedEnergy : CombatActivity
{
    public override IEnumerator Execute(CombatActivitySetting data)
    {
        Dictionary<EnergyType, int> currentEnergy = new Dictionary<EnergyType, int>();
        foreach (EnergyType e in CombatSystem.instance.energyMax.Keys)
            currentEnergy[e] = -CombatSystem.instance.GetEnergy(e);

        CombatSystem.instance.ModifyEnergy(currentEnergy);
        yield return null;
    }

    public override string GetDescription(CombatActivitySetting data) => "Remove all saved energy";

    public override string GetToolTip(CombatActivitySetting data) => string.Empty;
}
