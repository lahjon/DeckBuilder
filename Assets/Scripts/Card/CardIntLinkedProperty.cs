using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CardIntLinkedProperty: CardInt
{
    Action onLinkedValChange;

    public override int value
    {
        set => base.value = value;
        get => _value + (PropGetter == null ? 0 : PropGetter());
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
        value = 0;

        CardLinkablePropertyType prop;
        Enum.TryParse(input.Split(':')[1], out prop);
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
            default:
                return null;
        }
    }

    private int ValueGetterSizeHand() => CombatSystem.instance.Hand.Count - (card is CardCombat c ? (CombatSystem.instance.Hand.Contains(c) ? 1 : 0) : 0);
    private static int ValueGetterSizeDeck() => CombatSystem.instance.Hero.deck.Count;
    private static int ValueGetterSizeDiscard() => CombatSystem.instance.Hero.discard.Count;


    public override string GetTextForValue()
    {
        string retstring = "";
        switch (propertyType)
        {
            case CardLinkablePropertyType.Handsize:
                retstring += "for each card in your hand";
                break;
            case CardLinkablePropertyType.NrCardsDeck:
                retstring += "for each card in your deck";
                break;
            case CardLinkablePropertyType.NrCardsDiscard:
                retstring += "for each card in your discard pile";
                break;
        }

        retstring += " </i>(" + value.ToString() + ")</i>";

        return retstring;
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

}

