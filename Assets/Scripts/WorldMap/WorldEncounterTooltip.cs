using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldEncounterTooltip : MonoBehaviour
{
    public TMP_Text difficultyText;
    public TMP_Text encounterName;
    public TMP_Text descriptionText;
    Reward encounterReward;
    public Transform rewardAnchor;
    public void EnableTooltip(WorldEncounter worldEncounter)
    {
        gameObject.SetActive(true);
        if (encounterName.text != worldEncounter.worldEncounterData.worldEncounterName)
        {
            if (encounterReward != null) Destroy(encounterReward.gameObject);

            // descriptionText = worldEncounter.worldEncounterData.clearCondition.
            encounterName.text = worldEncounter.worldEncounterData.worldEncounterName;
            encounterReward = Instantiate(worldEncounter.encounterReward, rewardAnchor).GetComponent<Reward>();
            encounterReward.transform.localPosition = Vector3.zero;

            transform.position = worldEncounter.transform.position + new Vector3(0, encounterReward.GetComponent<RectTransform>().sizeDelta.x + 1, 0);;

            encounterReward.transform.localScale *= 1.85f;
            difficultyText.text = worldEncounter.worldEncounterData.difficulty.ToString();
            encounterReward.gameObject.SetActive(true);
        }
    }

    public void DisableTooltip()
    {
        gameObject.SetActive(false);
    }
}
