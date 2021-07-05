using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RewardManager : Manager, IEvents
{
    public GameObject rewardPrefab;
    public Sprite[] icons;
    public RewardScreenCombat rewardScreenCombat;
    public RewardScreen rewardScreen;
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
    public void EnemyKilled(EnemyData enemyData)
    {
        rewardScreenCombat.callback = () => Enemy.EnemyKilledCallback(enemyData);
    }

    public void OpenDraftMode()
    {
        rewardScreenCardSelection.SetupRewards();
    }

    public void Subscribe()
    {
        EventManager.OnEnemyKilledEvent += EnemyKilled;
    }
    public void CreateRewards(RewardType[] rewards, Transform parent)
    {
        foreach (RewardType reward in rewards)
        {
            Reward newReward = Instantiate(WorldSystem.instance.rewardManager.rewardPrefab, parent).GetComponent<Reward>();
            newReward.SetupReward(reward);
        }
    }

    public void Unsubscribe()
    {
        EventManager.OnEnemyKilledEvent -= EnemyKilled;
    }
}
