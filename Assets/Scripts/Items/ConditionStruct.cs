using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ConditionStruct
{
    public ConditionType type;
    public string strParameter;
    public int numValue;


    public string GetDescription() => type switch
    {
        ConditionType.None => "No Condition",
        ConditionType.KillEnemy => string.Format("<b>Kill Enemies (" + numValue + ")</b>"),
        ConditionType.ClearTile => string.Format("<b>Clear Tiles (" + numValue + ")</b>"),
        ConditionType.WinCombat => string.Format("<b>Win Combats (" + numValue + ")</b>"),
        _ => null
    };
}
