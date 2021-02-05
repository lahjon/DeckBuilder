using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCard : EventMain
{
    private List<GameObject> cards = new List<GameObject>();

    public override bool TriggerEvent()
    {
        WorldSystem.instance.uiManager.rewardScreen.rewardScreenCard.GetComponent<RewardScreenCard>().SetupRewards(WorldSystem.instance.uiManager.encounterUITest.encounterData.cardData);
        base.TriggerEvent();
        return true;
    }
}
