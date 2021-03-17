using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : Manager
{
    public List<CharacterData> characterDatas = new List<CharacterData>();
    public DialogueData dialogueData;
    [HideInInspector]
    public Sprite currentArtwork;
    [HideInInspector]
    public string currentSentence;
    public Dialogue dialogue;
    int index;
    public bool activeDialogue;
    List<DialogueParticipant> players = new List<DialogueParticipant>();
    protected override void Awake()
    {
        base.Awake();
        world.dialogueManager = this;

        players.Add(DialogueParticipant.Beastmaster);
        players.Add(DialogueParticipant.Brute);
        players.Add(DialogueParticipant.Rogue);
        players.Add(DialogueParticipant.Splicer);
    }

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (activeDialogue && dialogue.sentenceDone && Input.GetMouseButtonDown(0))
        {
            NextSentence();
        }
        else if (activeDialogue && !dialogue.sentenceDone && Input.GetMouseButtonDown(0))
        {
            FinishSentence();
        }
    }

    CharacterData GetParticipantData(DialogueParticipant aParticipant)
    {
        for (int i = 0; i < characterDatas.Count; i++)
        {
            if (characterDatas[i].dialogueParticipant == aParticipant)
            {
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
            Debug.Log(aCharacterData.name);
            currentSentence = dialogueData.sentences[index].sentence;
            dialogue.SetUI(aCharacterData.artwork, aCharacterData.characterName, players.Contains(aCharacterData.dialogueParticipant));
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
        if (dialogueData != null && dialogueData.sentences.Count > 0 && !activeDialogue)
        {
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
    }

}
