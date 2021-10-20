using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "CardGame/Equipment")]
public class EquipmentData : ScriptableObject
{
    public int id;
    public string equipmentName;
    [TextArea(5, 5)]public string description;

    public int level;
    public EquipmentType equipmentType;
    public CharacterClassType classType;
    public Sprite artwork;
    public string effect;
}