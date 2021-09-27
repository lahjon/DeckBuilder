using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewMissionData", menuName = "CardGame/MissionData")]
public class MissionData : ProgressionData
{
    public bool mainMission;
    public string startEvent;
    public string endEvent;
    public MissionData nextMission;

}