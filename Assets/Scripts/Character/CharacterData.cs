using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class CharacterData : ScriptableObject
{
    public Sprite artwork;
    public GameObject artworkAnimated;
    public int damageModifier;
    public int blockModifier;
    public int drawCardsAmount;
    public int energy;
    public int maxHp;
    public int level;
    public CharacterClassType classType;
    public bool unlocked;


    public CardDatabase startingDeck;
    [TextArea(5,5)]
    public string description;
}
