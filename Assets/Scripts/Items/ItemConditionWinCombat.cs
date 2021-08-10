using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemConditionWinCombat : ItemCondition, IEvents
{
    public ItemConditionWinCombat(string aValue, UseItem anItem) : base(aValue, anItem)
    {
        //requiredAmount = int.Parse(aValue);
    }

    void WinCombat()
    {
        currentAmount++;
        CheckCondition();
    }

    public override void Subscribe()
    {
        EventManager.OnWinCombatEvent += WinCombat;
        Debug.Log("Sub");
    }

    public override void Unsubscribe()
    {
        EventManager.OnWinCombatEvent -= WinCombat;
        Debug.Log("Desub");
    }
}