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
        get => Mathf.Clamp(
            baseVal + modifier + (int)((PropGetter == null ? 0 : PropGetter())*Mathf.Pow(scalar, inverseScalar ? -1 : 1)),
            limitLower,
            limitUpper);
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
    public override CardLinkablePropertyType propertyType
    {
        get { return _propertyType; }
        set
        {
            _propertyType = value;
            PropGetter = GetGetter(value);
        }
    }

    public CardIntLinkedProperty(CardIntData data, Card card, Action onLinkedValChange = null)
    {
        baseVal = data.baseVal;
        scalar = data.scalar;
        inverseScalar = data.inverseScalar;

        if (card is CardCombat) // this shouldnt be necessary if player cards are not used for enemies. But is as of now
        {
            propertyType = data.linkedProp;
            card.registeredSubscribers.Add(this);
            this.card = card;
            this.onLinkedValChange = onLinkedValChange;
        }
        else
            _propertyType = data.linkedProp;
    }

    private Func<int> PropGetter;

    protected void ActionFire() => onLinkedValChange?.Invoke();

    public override void AbsorbModifier(CardIntData data)
    {
        baseVal += data.baseVal;    //Add val
        scalar = data.scalar;       //OVERwrite scalar. 
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
            case CardLinkablePropertyType.CountPlayedCardsSameName:
                return ValueGetterSameName;
            default:
                return null;
        }
    }

    private int ValueGetterSizeHand() => CombatSystem.instance.Hand.Count - (card is CardCombat c ? (CombatSystem.instance.Hand.Contains(c) ? 1 : 0) : 0);
    private static int ValueGetterSizeDeck() => CombatSystem.instance.Hero.deck.Count;
    private static int ValueGetterSizeDiscard() => CombatSystem.instance.Hero.discard.Count;
    private static int ValueGetterEnergy() => CombatSystem.instance.cEnergy;
    private int ValueGetterSpentEnergyCard() => card.cost.PaidEnergy;
    private int ValueGetterSameName() => CombatSystem.instance.playHistory.Count(x=> x.cardId == card.cardId);


    public override string GetTextForValue()
    {
        if (propertyType == CardLinkablePropertyType.CountPlayedCardsSameName)
            return base.GetTextForValue();

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

        if(card is CardCombat) //Can only be calculated in combat
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
            case CardLinkablePropertyType.CountPlayedCardsSameName:
                EventManager.OnCardPlayNoArgEvent += ActionFire;
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
            case CardLinkablePropertyType.CountPlayedCardsSameName:
                EventManager.OnCardPlayNoArgEvent -= ActionFire;
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

