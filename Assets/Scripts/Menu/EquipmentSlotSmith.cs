using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlotSmith : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public EquipmentData equipmentData;
    public Image image;
    public int level;
    public void BindData(EquipmentData data, int aLevel)
    {
        if (data == null) return;
        level = aLevel;

        if (!image.enabled) image.enabled = true;
        equipmentData = data;
        image.sprite = data.artwork;
    }

    public void ButtonUpgrade()
    {
        WorldSystem.instance.townManager.smith.Upgrade(equipmentData.equipmentType);
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
