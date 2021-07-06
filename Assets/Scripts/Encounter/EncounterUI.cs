using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class EncounterUI : MonoBehaviour
{
    public TMP_Text encounterTitle;
    public TMP_Text encounterDescription;
    public GameObject canvas;
    public CanvasGroup background;
    public GameObject[] choices;
    public EncounterDataRandomEvent encounterData;
    public EncounterDataRandomEvent newEncounterData;
    private bool transition = false;

    public void StartEncounter()
    {
        canvas.SetActive(true);
        BindEncounterData();
    }

    public void BindEncounterData()
    {
        encounterTitle.text = encounterData.encounterName;
        encounterDescription.text = encounterData.description;

        foreach (GameObject go in choices)
            go.SetActive(false);

        for (int i = 0; i < encounterData.choices.Count; i++)
        {
            choices[i].SetActive(true);
            choices[i].transform.GetChild(0).GetComponent<TMP_Text>().text = encounterData.choices[i].label;
        }
    }

    //Button option clicked
    public void ChooseOption(int index)
    {
        if (transition) return;

        foreach(EncounterEventEffectStruct effectStruct in encounterData.choices[index - 1].effects)
            ExecuteEffect(effectStruct);

        switch (encounterData.choices[index - 1].outcome)
        {
            case EncounterEventChoiceOutcome.NewEvent:
                newEncounterData = (EncounterDataRandomEvent)encounterData.choices[index - 1].newEncounter;
                StartCoroutine(FadeToNextEvent());
                return;
            case EncounterEventChoiceOutcome.Combat:
                CombatSystem.instance.encounterData = (EncounterDataCombat)encounterData.choices[index - 1].newEncounter;
                WorldStateSystem.SetInCombat(true);
                break;
            default:
                WorldStateSystem.SetInEvent(false);
                break;
        }
        CloseEncounter();
    }

    IEnumerator FadeToNextEvent()
    {
        transition = true;
        float time = 0.3f;

        for (float t = time; t >= 0; t -= Time.deltaTime)
        {
            background.alpha = t / time;
            yield return null;
        }
        encounterData = newEncounterData;
        newEncounterData = null;
        BindEncounterData();

        for (float t = time; t >= 0; t -= Time.deltaTime)
        {
            background.alpha = Mathf.Abs(1 - (t / time));
            yield return null;
        }
        transition = false;
    }
    public void CloseEncounter()
    {
        canvas.SetActive(false);
    }

    public void ExecuteEffect(EncounterEventEffectStruct effectStruct)
    {
        int x;
        switch (effectStruct.effect){
            case EncounterEventChoiceEffect.LifeCurrent:
                x = int.Parse(effectStruct.parameter);
                if (x > 0)      WorldSystem.instance.characterManager.Heal(x);
                else            WorldSystem.instance.characterManager.TakeDamage(-x);
                break;
            case EncounterEventChoiceEffect.LifeMax:
                x = int.Parse(effectStruct.parameter);
                WorldSystem.instance.characterManager.characterStats.ModifyHealth(x);
                break;
            case EncounterEventChoiceEffect.Artifact:
                ArtifactManager artifactManager = WorldSystem.instance.artifactManager;
                artifactManager.AddArtifact(artifactManager.GetRandomAvailableArtifact()?.name);
                break;
            case EncounterEventChoiceEffect.Card:
                CardClassType cardClassType = (CardClassType)WorldSystem.instance.characterManager.selectedCharacterClassType;
                WorldSystem.instance.characterManager.AddCardDataToDeck(DatabaseSystem.instance.GetRandomCard(cardClassType));
                break;
            case EncounterEventChoiceEffect.Gold:
                x = int.Parse(effectStruct.parameter);
                WorldSystem.instance.characterManager.gold += x;
                break;
        }
    }
}
