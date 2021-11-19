using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewMissionData", menuName = "CardGame/MissionData")]
public class MissionData : ProgressionData
{
    public bool mainMission;
    public List<GameEventStruct> gameEventsOnStart = new List<GameEventStruct>();
    public List<GameEventStruct> gameEventsOnEnd = new List<GameEventStruct>();
    public List<int> addDialogueIdx = new List<int>();
    public int nextMissionId;
}