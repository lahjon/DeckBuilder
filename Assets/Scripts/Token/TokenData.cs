using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewTokenData", menuName = "CardGame/TokenData")]
public class TokenData : ScriptableObject
{
    public string tokenName;
    public int tokenId;
    public int cost;

    [TextArea(5,5)]
    public string description;
    public Rarity rarity;
    public Sprite artwork;
    public string effectName;
}
