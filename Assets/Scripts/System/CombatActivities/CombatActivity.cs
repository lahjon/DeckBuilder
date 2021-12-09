using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatActivity
{
    public static CombatActivitySystem ActivitySystem;

    public abstract IEnumerator Execute(CombatActivitySetting data);
    public abstract string GetDescription(CombatActivitySetting data);
    public abstract string GetToolTip(CombatActivitySetting data);

}
