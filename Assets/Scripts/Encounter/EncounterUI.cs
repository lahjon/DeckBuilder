using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class EncounterUI : MonoBehaviour
{
    public TMP_Text encounterTitle;
    public GameObject canvas;
    public CanvasGroup background;
    public EncounterDataRandomEvent encounterData;
    public EncounterDataRandomEvent newEncounterData;
    public List<EncounterEventReferences> references = new List<EncounterEventReferences>();

    private bool transition = false;


    public void StartEncounter()
    {
        canvas.SetActive(true);
        BindEncounterData();
    }

    public void BindEncounterData()
    {
        encounterTitle.text = encounterData.encounterName;

        references.ForEach(r => r.gameObject.SetActive(false));

        EncounterEventReferences reference = references.Where(r => r.layoutType == encounterData.layoutType).First();
        reference.gameObject.SetActive(true);
        reference.encounterDescription.text = encounterData.description;

        foreach (GameObject go in reference.choices)
            go.SetActive(false);

        for (int i = 0; i < encounterData.choices.Count; i++)
        {
            reference.choices[i].SetActive(true);
            reference.choices[i].transform.GetChild(0).GetComponent<TMP_Text>().text = encounterData.choices[i].label;
        }

        if(encounterData.art != null)
            reference.image.sprite = encounterData.art;
    }

    //Button option clicked
    public void ChooseOption(int index)
    {
        if (transition) return;
        List<bool> rewards = new List<bool>();

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
                WorldStateSystem.SetInEventReward(true);
                break;
        }
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
            case EncounterEventChoiceEffect.Gold:
                x = int.Parse(effectStruct.parameter);
                WorldSystem.instance.characterManager.characterCurrency.gold += x;
                break;
            case EncounterEventChoiceEffect.Artifact:
                WorldSystem.instance.rewardManager.CreateRewardCombat(RewardCombatType.Artifact, null);
                break;
            case EncounterEventChoiceEffect.GetCards:
                CardClassType cardClassType = (CardClassType)WorldSystem.instance.characterManager.selectedCharacterClassType;
                WorldSystem.instance.rewardManager.CreateRewardCombat(RewardCombatType.Card, effectStruct.parameter);
                break;
        }
    }
}
