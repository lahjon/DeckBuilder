using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class CharacterData : ScriptableObject
{
    public string charName;
    public Sprite artwork;
    public CharacterType characterType;

    public List<StatModifer> stats;
}
