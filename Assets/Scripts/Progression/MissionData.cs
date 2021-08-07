using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewMissionData", menuName = "CardGame/MissionData")]
public class MissionData : ProgressionData
{
    [TextArea(5,5)]public string description;
    public string startEvent;
    public string endEvent;
    public string nextMission;

}