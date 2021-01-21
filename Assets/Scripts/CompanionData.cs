using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewCompanion", menuName = "Companion")]
public class CompanionData : ScriptableObject
{
    public string companionName;
    public int goldValue;
    public Sprite artwork;
    public Rarity rarity;
}
