using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class Character : ScriptableObject
{
    public new string name;
    public Sprite artwork;
    public CharacterType characterType;

    public int strength;    // card damage
    public int cunning;     // skill power 
    public int speed;       // cards to draw
    public int endurance;   // action points
    public int wisdom;      // deck size

    
}
