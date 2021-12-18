using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEffectAdder
{
    public EquipmentData equipmentData;
    public Image image;
    public ItemEffect itemEffect;
    public int level;
    public void BindData(EquipmentData data, int aLevel, bool addEffect)
    {
        if (data == null) return;
        level = aLevel;

        if (!image.enabled) image.enabled = true;
        equipmentData = data;
        image.sprite = data.artwork;
        if (addEffect)
        {
            itemEffect?.DeRegister();
            itemEffect = ItemEffect.Factory(equipmentData.itemEffectStruct, this);
            itemEffect?.Register();
        }
    }

    public string GetName()
    {
        return equipmentData.itemName;
    }
    public int GetValue() => itemEffect.itemEffectStruct.value;

    public void NotifyUsed()
    {
        
    }

    public void NotifyDeregister()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        WorldSystem.instance.menuManager.menuCharacter.ActivateToolTip(equipmentData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        WorldSystem.instance.menuManager.menuCharacter.DeactivateToolTip();
    }

    public void RemoveEffect()
    {
    }

    public void NotifyRegister()
    {
    }
}
