using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConditionData
{
    public ConditionType type;
    public string strParameter;
    public int numValue;

    public List<string> strParameters = new List<string>();
}

public static class ConditionStructExtensions
{
    public static string GetMissionDescription(this ConditionData condition, int cAmount, int rAmount)
    {
        //Debug.Log(rAmount);
        switch (condition.type)
        {
            case ConditionType.EnterBuilding:
                return rAmount != 0 ? string.Format("Enter {0} - ({1} / {2})", condition.strParameter, cAmount.ToString(), rAmount.ToString()) : "Completed";
            case ConditionType.KillBoss:
                return string.Format("Kill {0}", condition.strParameter);
            default:
                Debug.LogWarning(string.Format("Implement a new description for type: {0}", condition.type));
                return string.Format("Implement a new description for type: {0}", condition.type);
        }
    }
}

