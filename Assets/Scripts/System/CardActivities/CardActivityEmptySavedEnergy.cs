
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardActivityEmptySavedEnergy : CardActivity
{
    public override IEnumerator Execute(CardActivitySetting data)
    {
        Dictionary<EnergyType, int> currentEnergy = new Dictionary<EnergyType, int>();
        foreach (EnergyType e in CombatSystem.instance.energyMax.Keys)
            currentEnergy[e] = -CombatSystem.instance.GetEnergy(e);

        CombatSystem.instance.ModifyEnergy(currentEnergy);
        yield return null;
    }

    public override string GetDescription(CardActivitySetting data) => "Remove all saved energy";

    public override string GetToolTip(CardActivitySetting data) => string.Empty;
}
