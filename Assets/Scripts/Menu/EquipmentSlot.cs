using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class EquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] Equipment _equipment;
    public bool IsEmptySlot => Equipment == null;
    public static MenuInventory menuInventory;
    public virtual Equipment Equipment
    {
        get => _equipment;
        set
        {
            _equipment = value;
            if (_equipment != null)
            {
                _equipment.equipmentSlot = this;
                _equipment.ResetEquipment();
            }
        }
    }
    void Start()
    {
        if (menuInventory == null) menuInventory = WorldSystem.instance.menuManager.menuInventory;
    }
    void LeftMouseClick()
    {
        Debug.Log("Left");
    }
    protected virtual void RightMouseClick()
    {
        InventoryToGear(this, menuInventory.gearSlots.First(x => x.equipmentType == Equipment.equipmentDataStruct.equipmentType));
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_equipment != null)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                LeftMouseClick();
            else if (eventData.button == PointerEventData.InputButton.Right)
                RightMouseClick();
        }
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        menuInventory.currentHoveredEquipmentSlot = this;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (menuInventory.currentHoveredEquipmentSlot == this) menuInventory.currentHoveredEquipmentSlot = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Equipment != null && eventData.button == PointerEventData.InputButton.Left)
        {
            menuInventory.currentHoveredEquipment.transform.position = Input.mousePosition;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Equipment != null && eventData.button == PointerEventData.InputButton.Left)
        {
            Equipment.transform.SetParent(menuInventory.dragParent);
            menuInventory.currentHoveredEquipment = Equipment;
        }
    }

    void InventoryToInventory(EquipmentSlot source, EquipmentSlot target)
    {
        if (target.IsEmptySlot)
        {
            target.Equipment = Equipment;
            source.Equipment = null;
        }
        else
        {
            Equipment tempEquipment = target.Equipment;
            target.Equipment = Equipment;
            source.Equipment = tempEquipment;
        }

    }

    protected void GearToInventory(GearSlot source, EquipmentSlot target)
    {
        Equipment tempEquipment = Equipment;
        target = target.Equipment == null ? target : menuInventory.RequestInventorySpace();
        if (target != null && source.UnequipGear(target)) 
            target.Equipment = tempEquipment;
    }
    void InventoryToGear(EquipmentSlot source, GearSlot target)
    {
        if (target.IsCorrectGearType(Equipment)) 
        {
            if (target.IsEmptySlot)
            {
                target.EquipGear(Equipment);
                source.Equipment = null;
            }
            else
            {
                Equipment tempEquipment = source.Equipment;
                target.UnequipGear(source);
                target.EquipGear(tempEquipment);
            }
        }
    }

    public virtual void ReleaseDrop(EquipmentSlot sourceSlot, EquipmentSlot targetSlot)
    {
        if (targetSlot != sourceSlot) // not same slot
        {
            if (targetSlot is GearSlot targetGearSlot) InventoryToGear(sourceSlot, targetGearSlot);
            else if (sourceSlot is GearSlot sourceGearSlot) GearToInventory(sourceGearSlot, targetSlot);
            else InventoryToInventory(sourceSlot, targetSlot);
        }
        Equipment?.ResetEquipment();
        menuInventory.currentHoveredEquipment = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Equipment != null && eventData.button == PointerEventData.InputButton.Left) ReleaseDrop(this, menuInventory.currentHoveredEquipmentSlot == null ? menuInventory.currentHoveredEquipment.equipmentSlot : menuInventory.currentHoveredEquipmentSlot);
    }
}

public enum DropType
{
    None,
    InventoryToGear,
    GearToInventory,
    InventoryToInventory
}
