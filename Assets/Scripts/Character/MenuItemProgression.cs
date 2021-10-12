using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class MenuItemProgression : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text itemText;
    public ProgressionData data;
    public Mission mission;
    public void SetMissionItem(Mission aMission)
    {
        data = aMission.data;
        mission = aMission;
        if (mission?.countingConditions.Any() != null)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            titleText.text =  mission.aName;
            for (int i = 0; i < data.conditionStructs.Count; i++)
            {
                sb.AppendLine(data.conditionStructs[i].GetMissionDescription(mission.countingConditions[i].currentAmount, mission.countingConditions[i].requiredAmount));
            }
            itemText.text = sb.ToString();
        }
        else
        {
            titleText.text = "Unknown";
            itemText.text = "Missing Data";
        }
    }
    public void SetMissionItem(ProgressionData aData)
    {
        data = aData;
        if (aData != null)
        {
            titleText.text = aData.aName;
            itemText.text = "Completed";
        }
        else
        {
            titleText.text = "Unknown";
            itemText.text = "Missing Data";
        }
    }

    public void ButtonClick()
    {
        MenuProgression mP = WorldSystem.instance.menuManager.menuProgression;
        if (mP.currentProgressionData != data)
        {
            mP.EnableDescription(data, mission);
        }
        else
        {
            mP.DisableDescription();
        }
    }
}
