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
    public void BindData(EquipmentData data)
    {
        if (data == null) return;

        if (!image.enabled) image.enabled = true;
        equipmentData = data;
        image.sprite = data.artwork;
        itemEffect = ItemEffect.Factory(equipmentData.itemEffectStruct, this);
    }

    public string GetName()
    {
        throw new System.NotImplementedException();
    }

    public int GetValue() => itemEffect.itemEffectStruct.value;

    public void NotifyUsed()
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
        throw new System.NotImplementedException();
    }
}
