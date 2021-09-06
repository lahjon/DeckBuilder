using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewProgressionData", menuName = "CardGame/ProgressionData")]
public class ProgressionData : ScriptableObject
{
    public string id;
    public string aName;
    public List<ConditionStruct> conditionStructs;

}