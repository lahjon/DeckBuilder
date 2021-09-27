using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class MenuItemProgression : MonoBehaviour
{
    public TMP_Text itemText;
    public ProgressionData data;
    public int cAmount = 0;
    public int rAmount = 0;

    public void SetObjectiveItem(Objective obj)
    {
        data = obj.data;
        if (obj?.countingConditions.Any() != null)
        {
            cAmount = obj.countingConditions[0].currentAmount;
            rAmount = obj.countingConditions[0].requiredAmount;
            itemText.text = string.Format("{0} - {1} ({2} / {3})", obj.aName, obj.countingConditions[0].conditionStruct.type.ToString(), cAmount, rAmount); 
        }
        else
        {
            itemText.text = "Missing Data";
        }
    }
    public void SetObjectiveItem(ProgressionData aData)
    {
        data = aData;
        itemText.text = string.Format("{0} - Completed", aData.aName); 
    }

    public void SetMissionItem(Mission mission)
    {
        data = mission.data;
        if (mission?.countingConditions.Any() != null)
        {
            cAmount = mission.countingConditions[0].currentAmount;
            rAmount = mission.countingConditions[0].requiredAmount;
            itemText.text = string.Format("{0} - {1} ({2} / {3})", mission.aName, mission.countingConditions[0].conditionStruct.type.ToString(), cAmount, rAmount); 
        }
        else
        {
            itemText.text = "Missing Data";
        }
    }
    public void SetMissionItem(ProgressionData aData)
    {
        data = aData;
        itemText.text = string.Format("{0} - Completed", aData.aName); 
    }

    public void ButtonClick()
    {
        MenuProgression mP = WorldSystem.instance.menuManager.menuProgression;
        if (mP.currentProgressionData != data)
        {
            mP.EnableDescription(data, cAmount, rAmount);
        }
        else
        {
            mP.DisableDescription();
        }
    }
}
