using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CardIntLinkedProperty: CardInt
{
    Action onLinkedValChange;

    int scalar = 1;
    bool inverseScalar = false;

    public override int value
    {
        get => base.value + (int)((PropGetter == null ? 0 : PropGetter())*Mathf.Pow(scalar, inverseScalar ? -1 : 1));
    }

    private CalcType calcType
    {
        get
        {
            if (scalar == 1 && baseVal == 0)
                return CalcType.None;
            else if (scalar > 1)
                return inverseScalar ? CalcType.Dividing : CalcType.Multiplicative;
            else
                return CalcType.Additive;
        }
    }

    private CardLinkablePropertyType _propertyType = CardLinkablePropertyType.None;
    public CardLinkablePropertyType propertyType
    {
        get { return _propertyType; }
        set
        {
            _propertyType = value;
            PropGetter = GetGetter(value);
        }
    }

    public CardIntLinkedProperty(string input, Card card, Action onLinkedValChange = null)
    {
        CardLinkablePropertyType prop;
        string[] parts = input.Split(new char[] { ':', '/', '*', '+' });
        for (int i = 0; i < parts.Length; i++)
            Debug.Log("Part " + i.ToString() + " : " + parts[i]);
        Enum.TryParse(parts[1], out prop);
        if(parts.Length == 3)
        {
            int numVal = int.Parse(parts[2]);
            if (input.Contains("+"))
                baseVal = numVal;
            else
            {
                scalar = numVal;
                inverseScalar = input.Contains("/");
            }
        }

        propertyType = prop;

        this.card = card;
        card.registeredSubscribers.Add(this);
        this.onLinkedValChange = onLinkedValChange;

    }
    private Func<int> PropGetter;
     
    private void ActionFire()
    {
        onLinkedValChange?.Invoke();
    }

    private Func<int> GetGetter(CardLinkablePropertyType prop)
    {
        switch (prop)
        {
            case CardLinkablePropertyType.Handsize:
                return ValueGetterSizeHand;
            case CardLinkablePropertyType.NrCardsDeck:
                return ValueGetterSizeDeck;
            case CardLinkablePropertyType.NrCardsDiscard:
                return ValueGetterSizeDiscard;
            case CardLinkablePropertyType.EnergyAvailable:
                return ValueGetterEnergy;
            case CardLinkablePropertyType.CardEnergySpent:
                return ValueGetterSpentEnergyCard;
            default:
                return null;
        }
    }

    private int ValueGetterSizeHand() => CombatSystem.instance.Hand.Count - (card is CardCombat c ? (CombatSystem.instance.Hand.Contains(c) ? 1 : 0) : 0);
    private static int ValueGetterSizeDeck() => CombatSystem.instance.Hero.deck.Count;
    private static int ValueGetterSizeDiscard() => CombatSystem.instance.Hero.discard.Count;
    private static int ValueGetterEnergy() => CombatSystem.instance.cEnergy;
    private int ValueGetterSpentEnergyCard() => ((CardCombat)card).energySpent;


    public override string GetTextForValue()
    {
        string retstring = "";
        CalcType calcType = this.calcType;


        if (calcType == CalcType.Dividing)
            retstring += "for every " + scalar.ToString() + " ";
        else
            retstring += "for each "; 

        switch (propertyType)
        {
            case CardLinkablePropertyType.Handsize:
                retstring += "card in your hand";
                break;
            case CardLinkablePropertyType.NrCardsDeck:
                retstring += "card in your deck";
                break;
            case CardLinkablePropertyType.NrCardsDiscard:
                retstring += "card in your discard pile";
                break;
        }

        if (calcType == CalcType.Multiplicative) retstring += " times " + scalar.ToString();
        else if (calcType == CalcType.Additive) retstring += " +" + baseVal.ToString();

        retstring += " </i>(" + value.ToString() + ")</i>";

        return retstring;
    }

    public override string GetTextForTimes()
    {
        switch (propertyType)
        {
            case CardLinkablePropertyType.CardEnergySpent:
                return "X";
            default:
                return "Only set energy spent as times!";
        }
    }

    public override string GetTextForCost()
    {
        switch (propertyType)
        {
            case CardLinkablePropertyType.EnergyAvailable:
                return "X";
            default:
                return "N/A";
        }
    }

    public override void Subscribe()
    {
        if (onLinkedValChange == null) return;
        switch (propertyType)
        {
            case CardLinkablePropertyType.Handsize:
                EventManager.OnHandCountChangeEvent += ActionFire;
                break;
            case CardLinkablePropertyType.NrCardsDeck:
                EventManager.OnDeckCountChangeEvent += ActionFire;
                break;
            case CardLinkablePropertyType.NrCardsDiscard:
                EventManager.OnDiscardCountChangeEvent += ActionFire;
                break;
        }
    }

    public override void Unsubscribe()
    {
        if (onLinkedValChange == null) return;
        switch (propertyType)
        {
            case CardLinkablePropertyType.Handsize:
                EventManager.OnHandCountChangeEvent -= ActionFire;
                break;
            case CardLinkablePropertyType.NrCardsDeck:
                EventManager.OnDeckCountChangeEvent -= ActionFire;
                break;
            case CardLinkablePropertyType.NrCardsDiscard:
                EventManager.OnDiscardCountChangeEvent -= ActionFire;
                break;
        }
    }


    private enum CalcType
    {
        None,
        Multiplicative, 
        Dividing, 
        Additive
    }

}

