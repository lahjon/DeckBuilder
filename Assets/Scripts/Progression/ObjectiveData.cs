using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewObjectiveData", menuName = "CardGame/ObjectiveData")]
public class ObjectiveData : ProgressionData
{
    [TextArea(5,5)]public string description;
    public ObjectiveData nextObjective;
    public string endEvent;
}