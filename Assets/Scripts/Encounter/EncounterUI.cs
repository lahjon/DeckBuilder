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
            case EncounterEventChoiceOutcome.CardRandom:
                WorldSystem.instance.rewardManager.rewardScreenCombat.rewardScreenCard.GetComponent<RewardScreenCardSelection>().SetupRewards();
                break;
            case EncounterEventChoiceOutcome.CardSpecific:
                WorldSystem.instance.rewardManager.rewardScreenCombat.rewardScreenCard.GetComponent<RewardScreenCardSelection>().SetupRewards(WorldSystem.instance.uiManager.encounterUI.encounterData.chosenOption.cardRewards);
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
}
