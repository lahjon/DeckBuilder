using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class Equipment : MonoBehaviour, IEffectAdder, IPointerEnterHandler, IPointerExitHandler
{
    public EquipmentDataStruct equipmentDataStruct;
    public EquipmentSlot equipmentSlot;
    public Image image;
    public List<ItemEffect> itemEffects = new List<ItemEffect>();
    public void BindData(EquipmentDataStruct aEquipmentDataStruct)
    {
        equipmentDataStruct = aEquipmentDataStruct;
    }
    public void ResetEquipment()
    {
        transform.SetParent(equipmentSlot.transform);
        transform.localPosition = Vector3.zero;
    }

    public void Equip()
    {

    }
    public void Unequip()
    {
        
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }

    public void NotifyDeregister()
    {
        throw new System.NotImplementedException();
    }

    public void NotifyRegister()
    {
        throw new System.NotImplementedException();
    }

    public void NotifyUsed()
    {
        throw new System.NotImplementedException();
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (equipment != null) WorldSystem.instance.menuManager.menuCharacter.ActivateToolTip(equipment.equipmentData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //WorldSystem.instance.menuManager.menuCharacter.DeactivateToolTip();
    }
}
