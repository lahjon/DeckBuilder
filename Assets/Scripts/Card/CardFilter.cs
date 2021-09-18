using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardFilter
{
    #nullable enable
    public string? name;
    public Rarity? rarity;
    public CardClassType? classType;
    public CardClassType? notClassType;
    public string? cost;
    public Rarity[]? rarityArr;
    public CardClassType[]? classTypeArr;
    public CardClassType[]? notClassTypeArr;
    public int[]? costArr;
    #nullable disable

    public static bool Filterer(CardData cd, CardFilter cf)
    {
        if (cf.name != null             && cf.name != cd.name)                                              return false;
        if (cf.notClassType != null     && cf.notClassType == cd.cardClass)                                 return false;
        if (cf.classType != null        && cf.classType != cd.cardClass)                                    return false;
        if (cf.rarity != null           && cf.rarity != cd.rarity)                                          return false;
        if (cf.notClassTypeArr != null  && System.Array.IndexOf(cf.notClassTypeArr, cd.cardClass) > -1)     return false;
        if (cf.classTypeArr != null     && System.Array.IndexOf(cf.classTypeArr, cd.cardClass) < 0)         return false;
        if (cf.costArr != null          && System.Array.IndexOf(cf.costArr, cd.cost) < 0)                   return false;
        if (cf.rarityArr != null        && System.Array.IndexOf(cf.classTypeArr, cd.rarity) < 0)            return false;

        return true;
    }
}
