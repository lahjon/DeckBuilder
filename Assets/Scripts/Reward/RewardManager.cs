using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class RewardManager : Manager, IEventSubscriber
{
    public GameObject rewardNormalPrefab, rewardCombatPrefab;
    public Sprite[] icons;
    public RewardScreenCombat rewardScreenCombat;
    [SerializeField] RewardScreen rewardScreen;
    public RewardScreenCardSelection rewardScreenCardSelection;
    public List<RewardCombat> uncollectedReward;
    public Transform rewardParent;
    protected override void Awake()
    {
        base.Awake();
        world.rewardManager = this;
    }
    public void OpenCombatRewardScreen()
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

    public void CollectRewards()
    {
        if (uncollectedReward?.Any() == true) 
        {
            OpenRewardScreen();
            rewardScreen.CollectReward(uncollectedReward[0]);
        }
        else  ClearRewardScreen();
    }

    public RewardCombat CreateRewardCombat(RewardCombatType type, string value = null, Transform parent = null)
    {
        if (parent == null) parent = rewardParent;

        RewardCombat reward = Instantiate(WorldSystem.instance.rewardManager.rewardCombatPrefab, parent).GetComponent<RewardCombat>();
        reward.gameObject.SetActive(false);
        reward.SetupReward(type, value);
        reward.AddReward();
        return reward;
    }
    public RewardNormal CreateRewardNormal(RewardNormalType type, string value = null, Transform parent = null)
    {
        if (parent == null) parent = rewardParent;

        RewardNormal reward = Instantiate(WorldSystem.instance.rewardManager.rewardNormalPrefab, parent).GetComponent<RewardNormal>();
        reward.gameObject.SetActive(false);
        reward.SetupReward(type, value);
        reward.AddReward();
        return reward;
    }

    public void TriggerReward()
    {
        WorldStateSystem.SetInTownReward(true);
    }

    public void CopyReward(RewardCombat aReward)
    {
        rewardScreen.CollectReward(aReward);
    }

    public void OpenRewardScreen()
    {
        Debug.Log("Open Reward Screen");
        rewardScreen.canvas.gameObject.SetActive(true);
    }

    public void ClearRewardScreen()
    {
        Debug.Log("Clear Reward Screen");
        if (rewardScreen.currentReward != null)
            Destroy(rewardScreen.currentReward.gameObject);
            
        WorldStateSystem.SetInTownReward(false);
        WorldStateSystem.SetInEventReward(false);

        rewardScreen.canvas.gameObject.SetActive(false);
    }

    public void Unsubscribe()
    {
        EventManager.OnEnemyKilledEvent -= EnemyKilled;
    }
}


[System.Serializable] public struct RewardNormalStruct
{
    public RewardNormalType type;
    public string value;
}