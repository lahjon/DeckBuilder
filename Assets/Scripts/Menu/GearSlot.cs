using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearSlot : EquipmentSlot, IToolTipable
{
    public EquipmentType equipmentType;
    public Transform tooltipAnchor;
    public Image gearImage;
    float width = 50;
    public bool EquipGear(Equipment equipment)
    {
        if (Equipment == null || menuInventory.AddEquipmentToInventory(Equipment))
        {
            menuInventory.usedInventorySlots--;
            equipment.Equip();
            Equipment = equipment;
            equipment.transform.SetParent(transform);
            equipment.transform.localPosition = Vector3.zero;
            return true;
        }
        else return false;
    }
    public bool UnequipGear(EquipmentSlot targetSlot)
    {
        if (Equipment == null) return true;
        else if (targetSlot != null && Equipment != null && menuInventory.AddEquipmentToInventory(Equipment, targetSlot))
        {
            Equipment.Unequip();
            Equipment = null;
            return true;
        }
        else 
            return false;
    }

    protected override void RightMouseClick()
    {
        GearToInventory(this, menuInventory.RequestInventorySpace());
    }

    public bool IsCorrectGearType(Equipment equipment) => equipment.equipmentDataStruct.equipmentType == equipmentType;

    public (List<string> tips, Vector3 worldPosition, float offset) GetTipInfo()
    {
        string desc = "";
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
        if (Equipment == null)
            desc = string.Format("<b>" + equipmentType.ToString() + " Slot</b>");
        return (new List<string>{desc} , pos, width);
    }

}
