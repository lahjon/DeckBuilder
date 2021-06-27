using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RewardManager : Manager, IEvents
{
    public RewardScreenCombat rewardScreenCombat;
    public RewardScreen rewardScreen;
    public System.Action callback;
    public RewardScreenCardSelection rewardScreenCardSelection;
    public int draftAmount = 0;
    public System.Action rewardCallback;
    protected override void Awake()
    {
        base.Awake();
        world.rewardManager = this;
    }


    public void OpenRewardScreen()
    {
        rewardScreenCombat.SetupRewards();
    }

    public void CloseRewardScreen()
    {
        if (callback != null)
        {
            callback.Invoke();
            callback = null;
        }
        rewardScreenCardSelection.canvas.SetActive(false);
        rewardScreenCombat.canvas.SetActive(false);
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
