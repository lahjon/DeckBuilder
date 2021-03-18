using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RewardManager : Manager
{
    public RewardScreen rewardScreen;
    public RewardScreenCardSelection rewardScreenCardSelection;
    public int draftAmount = 0;
    protected override void Awake()
    {
        base.Awake();
        world.rewardManager = this;
    }


    public void OpenRewardScreen()
    {
        rewardScreen.SetupRewards();
    }

    public void CloseRewardScreen()
    {
        rewardScreenCardSelection.canvas.SetActive(false);
        rewardScreen.canvas.SetActive(false);
    }

    public void OpenDraftMode()
    {
        rewardScreenCardSelection.SetupRewards();
    }

}
