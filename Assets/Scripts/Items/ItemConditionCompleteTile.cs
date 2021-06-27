using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemConditionCompleteTile : ItemCondition, IEvents
{
    public ItemConditionCompleteTile(string aValue, Item anItem) : base(aValue, anItem)
    {
        //requiredAmount = int.Parse(aValue);
    }

    void CompleteTile()
    {
        currentAmount++;
        CheckCondition();
    }
    public override void Subscribe()
    {
        EventManager.OnCompleteTileEvent += CompleteTile;
    }

    public override void Unsubscribe()
    {
        EventManager.OnCompleteTileEvent -= CompleteTile;
    }
}
