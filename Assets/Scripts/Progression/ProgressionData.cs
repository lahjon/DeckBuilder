using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewProgressionData", menuName = "CardGame/ProgressionData")]
public class ProgressionData : ScriptableObject
{
    public int id;
    public string aName;
    [TextArea(5,5)]public string description;
    public List<ConditionData> conditionStructs;

}