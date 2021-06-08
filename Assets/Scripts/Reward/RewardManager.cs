using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RewardManager : Manager, IEvents
{
    public RewardScreenCombat rewardScreen;
    public System.Action callback;
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
        if (callback != null)
        {
            callback.Invoke();
            callback = null;
        }
        rewardScreenCardSelection.canvas.SetActive(false);
        rewardScreen.canvas.SetActive(false);
    }

    public void EnemyKilled(EnemyData enemyData)
    {
        callback = () => Enemy.EnemyKilledCallback(enemyData);
    }

    public void OpenDraftMode()
    {
        rewardScreenCardSelection.SetupRewards();
    }

    public void Subscribe()
    {
        EventManager.OnEnemyKilledEvent += EnemyKilled;
    }

    public void Unsubscribe()
    {
        EventManager.OnEnemyKilledEvent -= EnemyKilled;
    }
}
