using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewObjectiveData", menuName = "CardGame/ObjectiveData")]
public class ObjectiveData : ProgressionData
{
    public ObjectiveData nextObjective;
    public GameEventStruct endEvent;
}