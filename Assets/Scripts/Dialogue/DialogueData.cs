using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "CardGame/Dialogue")]
public class DialogueData : ScriptableObject
{
    public List<Sentence> sentences = new List<Sentence>();
    public string startEvent;
    public string endEvent;
}
