using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
Attacker, 
Defender
};

public enum CardRarity
{
Common, 
Uncommon, 
Rare
};

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public new string name;
    public string description;

    public Sprite artwork;
    
    public int cost;
    public int damage;
    public int block;
    public CardRarity cardRarity;

    public CardType cardType;
}
