using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueManager : Manager, ISaveableWorld
{
    public List<CharacterData> characterDatas = new List<CharacterData>();

    public List<DialogueData> allDialogueData = new List<DialogueData>();
    public int currentDialogue;
    public DialogueData dialogueData;
    [HideInInspector]
    public Sprite currentArtwork;
    [HideInInspector]
    public string currentSentence;
    public Dialogue dialogue;
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
        if(currentDialogue == 0)
            dialogueData = allDialogueData[0];
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

    public void SetDataFromString(string dialogueName)
    {
        Debug.Log(allDialogueData[0].name);
        Debug.Log(allDialogueData[1].name);
        DialogueData data = allDialogueData.Where(x => x.name == dialogueName).FirstOrDefault();
        if (data != null) dialogueData = data;
    }

    public void StartDialogue()
    {
        
        if (dialogueData != null && dialogueData.sentences.Any() && !activeDialogue)
        {
            if (!string.IsNullOrEmpty(dialogueData.startEvent))
                WorldSystem.instance.gameEventManager.StartEvent(dialogueData.startEvent);

            WorldStateSystem.SetInDialogue();
            dialogue.gameObject.SetActive(true);
            activeDialogue = true;
            NextSentence();
        }
        else
        {
            Debug.Log("No dialogue or no sentences");
        }
    }

    public void EndDialogue()
    {
        index = 0;
        activeDialogue = false;
        dialogue.gameObject.SetActive(false);
        currentDialogue++;
        WorldStateSystem.TriggerClear();
        if (!string.IsNullOrEmpty(dialogueData.endEvent))
            WorldSystem.instance.gameEventManager.StartEvent(dialogueData.endEvent);
        dialogueData = null;
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.completedDialogue = currentDialogue;
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        currentDialogue = a_SaveData.completedDialogue;
    }
}
