using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Playable Character Data", menuName = "CardGame/PlayableCharacterData")]
public class PlayableCharacterData : CharacterData
{
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
}
