using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueManager : Manager, ISaveableWorld
{
    public List<CharacterData> characterDatas = new List<CharacterData>();

    public List<int> completedDialogues = new List<int>();
    public List<int> availableDialogues = new List<int>();
    [HideInInspector]
    public string currentSentence;
    public Dialogue dialogue;
    public DialogueData dialogueData;
    public int index;
    public bool activeDialogue;
    protected override void Awake()
    {
        base.Awake();
        world.dialogueManager = this;
    }

    protected override void Start()
    {
        base.Start();
    }

    public void AddDialogue(int idx)
    {
        if (!availableDialogues.Contains(idx) && !completedDialogues.Contains(idx))
            availableDialogues.Add(idx);
    }

    public void CompleteDialogue()
    {
        if (dialogueData != null)
        {
            availableDialogues.Remove(dialogueData.index);
            completedDialogues.Add(dialogueData.index);
        }
    }

    CharacterData GetParticipantData(DialogueParticipant aParticipant)
    {
        int idx = (int)aParticipant;
        if (idx <= 6)
            return world.characterManager.character.characterData;
        
        else
        {
            for (int i = 0; i < characterDatas.Count; i++)
            {
                if (characterDatas[i].dialogueParticipant == aParticipant)
                    return characterDatas[i];
            }
        }
        return null;
    }

    public void NextSentence()
    {
        if (index < dialogueData.sentences.Count)
        {
            CharacterData aCharacterData = GetParticipantData(dialogueData.sentences[index].dialogueParticipant);
            currentSentence = dialogueData.sentences[index].sentence;
            dialogue.SetUI(aCharacterData.artwork, aCharacterData.characterName, dialogueData.sentences[index].dialogueParticipant);
            dialogue.DisplaySentence(currentSentence);
            index++;
        }
        else
        {
            EndDialogue();
        }
    }

    public void FinishSentence()
    {
        dialogue.EndSentence();
    }

    public void StartDialogue()
    {
        if (availableDialogues.Any() && DatabaseSystem.instance.dialogues.FirstOrDefault(x => x.index == availableDialogues[0]) is DialogueData aDialogueData)
        {
            dialogueData = aDialogueData; 
            if (!activeDialogue && dialogueData.stateToTrigger.Contains(WorldStateSystem.instance.currentWorldState))
            {
                WorldStateSystem.SetInDialogue(true);
                dialogue.gameObject.SetActive(true);
                activeDialogue = true;
                NextSentence();
            }
        }
    }

    public void EndDialogue()
    {
        index = 0;
        activeDialogue = false;
        dialogue.gameObject.SetActive(false);
        CompleteDialogue();
        WorldStateSystem.SetInDialogue(false);
        if (dialogueData.endEvent?.Any() == true)
            for (int i = 0; i < dialogueData.endEvent.Count; i++)
                GameEventManager.CreateEvent(dialogueData.endEvent[i]);

        dialogueData = null;
        StartDialogue();
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.completedDialogues = completedDialogues;
        a_SaveData.availableDialogues = availableDialogues;
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.completedDialogues = completedDialogues;
        a_SaveData.availableDialogues = availableDialogues;
    }
}
