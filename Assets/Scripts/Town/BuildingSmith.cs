using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class BuildingSmith : Building
{
    public GameObject roomMain, roomToken, roomEquipment;
    public GameObject confirmWindow;
    public TMP_Text partText;
    public EquipmentType selectedEquipment;
    public void ButtonEnterToken()
    {
        StepInto(roomToken);
    }
    public void ButtonEnterEquipment()
    {
        StepInto(roomEquipment);
    }
    public override void CloseBuilding()
    {
        base.CloseBuilding();
    }
    
    public override void EnterBuilding()
    {
        base.EnterBuilding();
        StepInto(roomMain);
    }

    public void ButtonConfirmUpgrade()
    {
        WorldSystem.instance.characterManager.characterCurrency.armorShard--;
        WorldSystem.instance.equipmentManager.UpgradeEquipment(selectedEquipment);
        ButtonCloseUpgrade();
    }

    public void ButtonCloseUpgrade()
    {
        selectedEquipment = EquipmentType.None;
        confirmWindow.SetActive(false);
    }
    public void Upgrade(EquipmentType equipmentType)
    {
        if (WorldSystem.instance.characterManager.characterCurrency.armorShard > 0)
        {
            if (WorldSystem.instance.equipmentManager.allEquipment[equipmentType].level < WorldSystem.instance.equipmentManager.maxUpgradeLevel)
            {
                confirmWindow.SetActive(true);
                selectedEquipment = equipmentType;
            }
            else
                WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Not enough armor shards!");
        }
        else 
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Not enough armor shards!");
    }
}
