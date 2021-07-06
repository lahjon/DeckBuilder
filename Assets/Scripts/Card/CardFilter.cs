using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardFilter
{
    #nullable enable
    public string? name;
    public Rarity? rarity;
    public int? cost;
    #nullable disable

    public static bool Filterer(CardData cd, CardFilter cf)
    {
        if (cf.name != null && cf.name != cd.name)          return false;
        if (cf.rarity != null && cf.rarity != cd.rarity)    return false;
        if (cf.cost != null && cf.cost != cd.cost)          return false;


        return true;
    }
}
