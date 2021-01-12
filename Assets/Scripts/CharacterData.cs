using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class CharacterData : ScriptableObject
{
    public string charName;
    public Sprite artwork;
    public CharacterType characterType;

    // stats are +/- modifiers to starting stats
    public int strength;    // card damage
    public int cunning;     // skill power 
    public int speed;       // cards to draw
    public int endurance;   // action points
    public int wisdom;      // deck size
}
