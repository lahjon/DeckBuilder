using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class MenuItemProgression : MonoBehaviour
{
    public TMP_Text itemText;
    public void SetItem(Progression progression)
    {
        Debug.Log(progression.countingConditions.Count);
        if (progression?.countingConditions.Any() != null)
        {
            itemText.text = string.Format("{0} - ({1} / {2})", progression.aName, progression.countingConditions[0].currentAmount, progression.countingConditions[0].requiredAmount); 
        }
        else
        {
            itemText.text = "Missing Data";
        }
    }
}
