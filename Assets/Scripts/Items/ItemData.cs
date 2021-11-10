using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public int itemId;
    public Sprite artwork;
    [TextArea(5,5)]
    public string description;
    public Rarity rarity;
    public ItemEffectStruct itemEffectStruct;
}
