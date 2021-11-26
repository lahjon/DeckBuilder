using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class CombatRewardManager : Manager, IEventSubscriber
{
    
    public RewardScreenCombat rewardScreenCombat;
    public GameObject rewardCombatPrefab;
    public Transform rewardParent;
    public RewardScreenCardSelection rewardScreenCardSelection;
    public List<RewardCombat> uncollectedReward;
    public RewardCombatStruct[] combatRewardNormal;
    public RewardCombatStruct[] combatRewardElite;
    public RewardCombatStruct[] combatRewardBoss;
    protected override void Awake()
    {
        base.Awake();
        world.combatRewardManager = this;
    }
    public void OpenCombatRewardScreen()
    {
        rewardScreenCombat.SetupRewards();
    }

    public void EnemyKilled(EnemyData enemyData)
    {
        rewardScreenCombat.callback = () => Enemy.EnemyKilledCallback(enemyData);
    }
    public RewardCombat CreateRewardCombat(RewardCombatType type, string value = null)
    {
        RewardCombat reward = Instantiate(rewardCombatPrefab, rewardParent).GetComponent<RewardCombat>();
        reward.gameObject.SetActive(false);
        reward.SetupReward(type, value);
        reward.AddReward();
        return reward;
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

[System.Serializable] public struct RewardCombatStruct
{
    public RewardCombatType type;
    public string value;
}
