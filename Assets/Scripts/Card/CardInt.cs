using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CardInt: IEventSubscriber
{
    protected Action onLinkedValChange;

    protected Card card;

    public int baseVal;

    public int modifier = 0;

    public virtual CardLinkablePropertyType propertyType { get => CardLinkablePropertyType.None; set { } }

    public virtual int value { get => baseVal + modifier; }

    public static implicit operator int(CardInt ci) => ci.value;

    public static CardInt Factory(string input, Card card, Action OnPreConditionUpdate = null)
    {
        if (!input.Contains(':'))
            return new CardInt(int.Parse(input), OnPreConditionUpdate);
        else
            return new CardIntLinkedProperty(ParseInput(input), card, OnPreConditionUpdate);
    }

    public static CardIntLinkData ParseInput(string input)
    {
        if (!input.Contains(':'))
            return new CardIntLinkData() { baseVal = int.Parse(input) };

        CardIntLinkData retData = new CardIntLinkData();
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

    protected void ActionFire()
    {
        onLinkedValChange?.Invoke();
    }

    public virtual void AbsorbModifier(CardIntLinkData data)
    {
        baseVal += data.baseVal;
        ActionFire();
    }

    public CardInt() { }

    public CardInt(int val, Action OnPreConditionUpdate = null)
    {
        baseVal = val;
        onLinkedValChange = OnPreConditionUpdate;
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

