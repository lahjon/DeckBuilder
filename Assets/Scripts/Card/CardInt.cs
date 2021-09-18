using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CardInt: IEventSubscriber
{
    protected Card card;

    public int baseVal;

    public int modifier = 0;

    public virtual CardLinkablePropertyType propertyType { get => CardLinkablePropertyType.None; set { } }

    public virtual int value { get => baseVal + modifier; }

    public static implicit operator int(CardInt ci) => ci.value;

    public static CardInt Factory(CardIntData input, Card card, Action OnPreConditionUpdate = null)
    {
        if (input.linkedProp == CardLinkablePropertyType.None)
            return new CardInt(input.baseVal);
        else
            return new CardIntLinkedProperty(input, card, OnPreConditionUpdate);
    }

    public static CardIntData ParseInput(string input)
    {
        if (!input.Contains(':'))
            return new CardIntData() { baseVal = int.Parse(input) };

        CardIntData retData = new CardIntData();
        string[] parts = input.Split(new char[] { ':', '/', '*', '+' });
        
        Enum.TryParse(parts[1], out retData.linkedProp);
        if (parts.Length == 3)
        {
            int numVal = int.Parse(parts[2]);
            if (input.Contains("+"))
                retData.baseVal = numVal;
            else
            {
                retData.scalar = numVal;
                retData.inverseScalar = input.Contains("/");
            }
        }

        return retData;
    }

    public virtual void AbsorbModifier(CardIntData data)
    {
        baseVal += data.baseVal;
    }

    public CardInt() { }

    public CardInt(int val)
    {
        baseVal = val;
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

