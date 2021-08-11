using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldMapConfirmWindow : MonoBehaviour
{
    public TMP_Text encounterName;
    public TMP_Text difficultyText;
    public GameObject encounterReward;
    public Transform rewardAnchor;
    public void OpenConfirmWindow(WorldEncounter worldEncounter)
    {
        gameObject.SetActive(true);
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
    public void CloseConfirmWindow()
    {
        gameObject.SetActive(false);
    }

    public void GenerateMap()
    {
        // use this to create the map
        WorldSystem.instance.gridManager.GenerateMap();
    }

    public void ButtonOnClick()
    {
        WorldStateSystem.instance.overrideTransitionType = TransitionType.EnterMap;
        WorldStateSystem.instance.transitionScreen.midCallback = () => GenerateMap();
        WorldStateSystem.SetInOverworld(true);
    }
}