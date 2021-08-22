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
    public void OpenConfirmWindow(WorldEncounter worldEncounter)
    {
        gameObject.SetActive(true);
        if (encounterName.text != worldEncounter.worldEncounterData.worldEncounterName)
        {
            if (encounterReward != null) Destroy(encounterReward.gameObject);

            encounterName.text = worldEncounter.worldEncounterData.worldEncounterName;
            encounterReward = Instantiate(worldEncounter.encounterReward, rewardAnchor).GetComponent<Reward>();
            encounterReward.SetWorldReward();
            encounterReward.transform.localPosition = Vector3.zero;
            encounterReward.transform.localScale *= 1.85f;
            difficultyText.text = worldEncounter.worldEncounterData.difficulty.ToString();
            encounterReward.gameObject.SetActive(true);
        }
    }
    public void CloseConfirmWindow()
    {
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        WorldSystem.instance.worldMapManager.currentWorldEncounter.condition.Subscribe();
        WorldStateSystem.instance.overrideTransitionType = TransitionType.EnterMap;
        DatabaseSystem.instance.ResetEncountersCombatToDraw(null);
        DatabaseSystem.instance.ResetEncountersEventToDraw();
        Helpers.DelayForSeconds(1f, () => WorldSystem.instance.gridManager.GenerateMap());
        WorldStateSystem.SetInOverworld(true);
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
            WorldSystem.instance.worldMapManager.currentWorldEncounter.OnConditionTrue();
            WorldSystem.instance.worldMapManager.UpdateMap();
        }
        
        CloseConfirmWindow();
    }
}
