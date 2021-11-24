using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSingleFieldProperty : ICardTextElement, IEquatable<CardSingleFieldProperty>
{
    public CardSingleFieldPropertyType type;

    public CardSingleFieldProperty(CardSingleFieldPropertyType type)
    {
        this.type = type;
    }

    public string GetElementText()
    {
        return "<b>" + type.ToString() + "</b>";
    }

    public static implicit operator CardSingleFieldPropertyType(CardSingleFieldProperty p) => p.type;


    public string GetElementToolTip()
    {
        switch (type)
        {
            case CardSingleFieldPropertyType.Immediate:
                return "<b>Immediate</b>\nThis card will play itself when you draw it.";
            case CardSingleFieldPropertyType.Unplayable:
                return "<b>Unplayable</b>\nThis card can not be played.";
            case CardSingleFieldPropertyType.Unstable:
                return "<b>Unstable</b>\nThis card will exhaust if it is still in hand at end of turn.";
            case CardSingleFieldPropertyType.Exhaust:
                return "<b>Exhaust</b>\nThis card disappears when played.";
            case CardSingleFieldPropertyType.Fortify:
                return "<b>Fortify</b>\nThe power of this Oath increases for every Oath already played with the same name.";
            case CardSingleFieldPropertyType.Inescapable:
                return "<b>Inescapable</b>\nThis card plays itself at the end of turn.";
            default:
                return "<b>" + type.ToString() + "</b>" + " Who knows?"; 
        }
    }

    public bool Equals(CardSingleFieldProperty other)
    {
        return type == other;
    }
}

