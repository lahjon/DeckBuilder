using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRandomCard : EventMain
{
    private List<GameObject> cards = new List<GameObject>();

    public override bool TriggerEvent()
    {
        WorldSystem.instance.uiManager.rewardScreen.rewardScreenCard.GetComponent<RewardScreenCard>().SetupRewards();
        base.TriggerEvent();
        return true;
    }
}
