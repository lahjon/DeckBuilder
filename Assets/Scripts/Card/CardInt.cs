using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CardInt: IEventSubscriber
{
    internal Card card;

    public int baseVal;

    public int modifier = 0;

    public virtual int value { get => baseVal + modifier; }

    public static implicit operator int(CardInt ci) => ci.value;

    public static CardInt Factory(string input, Card card, Action OnPreConditionUpdate = null)
    {
        if (!input.Contains(':'))
            return new CardInt(int.Parse(input));
        else
            return new CardIntLinkedProperty(input, card, OnPreConditionUpdate);
    }
    public CardInt() { }

    public CardInt(int val, Card card = null)
    {
        baseVal = val;
        this.card = card;
    }

    public virtual string GetTextForValue()
    {
        return value.ToString();
    }

    public virtual string GetTextForTimes()
    {
        return value.ToString();
    }

    public virtual string GetTextForCost()
    {
        return value.ToString();
    }


    public virtual void Subscribe()
    {

    }

    public virtual void Unsubscribe()
    {

    }
}

