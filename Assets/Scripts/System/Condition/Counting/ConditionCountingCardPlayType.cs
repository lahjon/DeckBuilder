using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConditionCountingCardPlayType : ConditionCounting
{
    public override void Subscribe() {
        base.Subscribe();
        EventManager.OnCardPlayEvent += CheckValid; 
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        EventManager.OnCardPlayEvent -= CheckValid;
    }

    public void CheckValid(Card card)
    {
        if (conditionData.strParameter == card.cardType.ToString())
            OnEventNotification();
    }

}