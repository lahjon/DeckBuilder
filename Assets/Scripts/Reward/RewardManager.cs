using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class RewardManager : Manager, IEventSubscriber
{
    public GameObject rewardPrefab;
    public Sprite[] icons;
    public RewardScreenCombat rewardScreenCombat;
    [SerializeField] RewardScreen rewardScreen;
    public RewardScreenCardSelection rewardScreenCardSelection;
    public List<Reward> uncollectedReward;
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

    public Reward CreateReward(RewardType type, string value = null, Transform parent = null, bool addReward = true)
    {
        if (parent == null) parent = rewardParent;

        Reward reward = Instantiate(WorldSystem.instance.rewardManager.rewardPrefab, parent).GetComponent<Reward>();
        reward.gameObject.SetActive(false);
        reward.SetupReward(type, value);
        if (addReward) reward.AddReward();
        return reward;
    }

    public void TriggerReward()
    {
        WorldStateSystem.SetInTownReward(true);
    }

    public void CopyReward(Reward aReward)
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
