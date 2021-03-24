using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Sentence
{
    public DialogueParticipant dialogueParticipant;
    [TextArea(3,3)]
    public string sentence;
}
