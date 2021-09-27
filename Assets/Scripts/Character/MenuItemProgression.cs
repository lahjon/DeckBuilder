using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class MenuItemProgression : MonoBehaviour
{
    public TMP_Text itemText;
    public ProgressionData data;
    public void SetObjectiveItem(Objective obj)
    {
        data = obj.data;
        if (obj?.countingConditions.Any() != null)
        {
            itemText.text = string.Format("{0} - {1} ({2} / {3})", obj.aName, obj.countingConditions[0].conditionStruct.type.ToString(), obj.countingConditions[0].currentAmount, obj.countingConditions[0].requiredAmount); 
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
            itemText.text = string.Format("{0} - {1} ({2} / {3})", mission.aName, mission.countingConditions[0].conditionStruct.type.ToString(), mission.countingConditions[0].currentAmount, mission.countingConditions[0].requiredAmount); 
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
}
