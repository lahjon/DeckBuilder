using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldMapConfirmWindow : MonoBehaviour
{
    public TMP_Text encounterName;
    public TMP_Text difficultyText;
    public Reward encounterReward;
    public Transform rewardAnchor;
    public void OpenConfirmWindow(ScenarioUI worldEncounter)
    {
        gameObject.SetActive(true);
        if (encounterName.text != worldEncounter.data.ScenarioName)
        {
            if (encounterReward != null) Destroy(encounterReward.gameObject);

            encounterName.text = worldEncounter.data.ScenarioName;
            encounterReward = Instantiate(worldEncounter.scenarioReward, rewardAnchor).GetComponent<Reward>();
            encounterReward.SetWorldReward();
            encounterReward.transform.localPosition = Vector3.zero;
            encounterReward.transform.localScale *= 1.85f;
            difficultyText.text = worldEncounter.data.difficulty.ToString();
            encounterReward.gameObject.SetActive(true);
        }
    }
    public void CloseConfirmWindow()
    {
        //WorldSystem.instance.worldMapManager.currentWorldScenarioData = null;
        //WorldSystem.instance.worldMapManager.currentScenarioUI = null;
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        WorldStateSystem.instance.overrideTransitionType = TransitionType.EnterMap;
        WorldSystem.instance.scenarioMapManager.scenarioData = WorldSystem.instance.worldMapManager.currentWorldScenarioData;
        WorldSystem.instance.scenarioMapManager.ResetEncounters();
        Helpers.DelayForSeconds(1f, () => WorldSystem.instance.scenarioMapManager.GenerateMap());
        WorldStateSystem.SetInOverworld();
    }

    public void ButtonOnClick()
    {
        if (!WorldSystem.instance.debugMode)
        {
            Confirm();
        }
        else
        {
            Debug.Log("Debug mode complete encounter");
            WorldSystem.instance.worldMapManager.CompleteScenario(false);
            WorldSystem.instance.worldMapManager.UpdateMap();
        }
        
        CloseConfirmWindow();
    }
}
