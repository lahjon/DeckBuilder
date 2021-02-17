using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest")][System.Serializable]
public class Quest : ScriptableObject
{
    public string questName;
    [TextArea(5,5)]
    public string description;
    public string startText;
    public string endText;
    public List<QuestGoal> goal = new List<QuestGoal>(); 
    public List<QuestStartAction> startingActions = new List<QuestStartAction>(); 
}