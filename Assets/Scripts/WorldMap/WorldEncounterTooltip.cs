using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldEncounterTooltip : MonoBehaviour
{
    public TMP_Text difficultyText;
    public TMP_Text encounterName;
    GameObject encounterReward;
    public Transform rewardAnchor;
    public void EnableTooltip(WorldEncounter worldEncounter)
    {
        gameObject.SetActive(true);
        transform.position = worldEncounter.transform.position;
        if (encounterName.text != worldEncounter.worldEncounterData.worldEncounterName)
        {
            if (encounterReward != null) Destroy(encounterReward);

            encounterName.text = worldEncounter.worldEncounterData.worldEncounterName;
            encounterReward = Instantiate(worldEncounter.encounterReward, rewardAnchor);
            encounterReward.transform.localPosition = Vector3.zero;
            encounterReward.transform.localScale *= 1.85f;
            difficultyText.text = worldEncounter.worldEncounterData.difficulty.ToString();
            encounterReward.SetActive(true);
        }
    }

    public void DisableTooltip()
    {
        gameObject.SetActive(false);
    }
}
