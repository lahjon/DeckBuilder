using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EncounterUI : MonoBehaviour
{
    public EncounterData encounterData;
    public TMP_Text encounterTitle;
    public TMP_Text encounterDescription;
    public Encounter encounter;

    public void UpdateUI()
    {
        if(encounterTitle != null)
            encounterTitle.text = encounterData.name;
        if(encounterDescription != null)
            encounterDescription.text = encounterData.description;
    }
    public virtual void ConfirmOutcome(int index)
    {
        WorldSystem.instance.characterManager.AddToCharacter(encounter.encounterData.encounterOutcome[index].type, encounter.encounterData.encounterOutcome[index].value);
        DestroyUI();
    }
    public void DestroyUI()
    {
        encounter.SetIsCleared();
    }
}
