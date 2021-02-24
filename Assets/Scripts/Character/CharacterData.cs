using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class CharacterData : ScriptableObject
{
    public string charName;
    //deprecate when we have models for animation
    public Sprite artwork;
    public GameObject characterGraphics;
    public CharacterClass characterClass;

    public List<StatModifer> stats;
    public int maxHealth;
    public int energy;
    public int cardDrawAmount;

    public CardDatabase startingDeck;
}
