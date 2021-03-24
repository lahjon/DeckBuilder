using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite artwork;
    public DialogueParticipant dialogueParticipant;
    [TextArea(5,5)]
    public string description;
}
