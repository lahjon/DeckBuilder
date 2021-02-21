using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterEvent
{
    /// <returns>bool disable: Returns a bool that is used to disable the UI.</returns>
    public static bool TriggerEvent(EncounterEventType encounterEventType)
    {
        switch (encounterEventType)
        {
            case EncounterEventType.Combat:
                return EventCombat();

            case EncounterEventType.CardRandom:
                return EventCardRandom();

            case EncounterEventType.CardSpecific:
                return EventCardSpecfic();

            case EncounterEventType.NewEvent:
                return EventNewEvent();

            case EncounterEventType.NewMap:
                return EventNewMap();
            
            default:
                return EventLeave();
        }
    }

    private static bool EventLeave()
    {
        WorldSystem.instance.worldStateManager.RemoveState(false);
        return true;
    }

    private static bool EventCombat()
    {
        WorldSystem.instance.worldStateManager.RemoveState(false);
        WorldSystem.instance.EnterCombat(WorldSystem.instance.uiManager.encounterUI.encounterData.enemyData);
        return true;
    }
    private static bool EventCardRandom()
    {
        WorldSystem.instance.uiManager.rewardScreen.rewardScreenCard.GetComponent<RewardScreenCard>().SetupRewards();
        return true;
    }
    private static bool EventCardSpecfic()
    {
        WorldSystem.instance.uiManager.rewardScreen.rewardScreenCard.GetComponent<RewardScreenCard>().SetupRewards(WorldSystem.instance.uiManager.encounterUI.encounterData.cardData);
        return true;
    }
    private static bool EventNewEvent()
    {
        EncounterUI ui = WorldSystem.instance.uiManager.encounterUI;
        GameObject oldUI = ui.CreateNewUI(ui.background, ui.panel);
        ui.encounterData = ui.newEncounterData;
        ui.BindEncounterData();
        ui.background.GetComponent<CanvasGroup>().alpha = 0.0f;

        ui.StartFade(oldUI, ui.background);
        return false;
    }

    private static bool EventNewMap()
    {
        WorldSystem.instance.encounterManager.GenerateMap(2,2,4);
        WorldSystem.instance.worldStateManager.AddState(WorldState.Overworld, true);
        return true;
    }


}
