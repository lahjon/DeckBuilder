using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New PerkData", menuName = "CardGame/PerkData")]
public class PerkData : ItemData
{
    public int level;
    public Sprite inactiveArtwork;
    public string effect;
    public CharacterClassType characterClassType;
}
