using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "CardGame/Dialogue")]
public class DialogueData : ScriptableObject
{
    public int index;
    public List<Sentence> sentences = new List<Sentence>();
    public List<WorldState> stateToTrigger = new List<WorldState>();
    public List<GameEventStruct> endEvent = new List<GameEventStruct>();
}
