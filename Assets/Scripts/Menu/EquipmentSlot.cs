using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        itemEffect = ItemEffectManager.CreateItemEffect(equipmentData.itemEffectStruct);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        WorldSystem.instance.menuManager.menuCharacter.ActivateToolTip(equipmentData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        WorldSystem.instance.menuManager.menuCharacter.DeactivateToolTip();
    }
}
