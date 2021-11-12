using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "CardGame/Equipment")]
public class EquipmentData : ItemData
{
    public int level;
    public EquipmentType equipmentType;
    public CharacterClassType classType;
}
