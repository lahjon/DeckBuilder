using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New PerkData", menuName = "CardGame/PerkData")]
public class PerkData : ScriptableObject
{
    public int perkId;
    public string perkName;
    [TextArea (5, 5)]public string description;
    public Sprite activeArtwork;
    public Sprite inactiveArtwork;
    public string effect;
    public CharacterClassType characterClassType;
}
