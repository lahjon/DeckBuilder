using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RewardManager : Manager, IEventSubscriber
{
    public GameObject rewardPrefab;
    public Sprite[] icons;
    public RewardScreenCombat rewardScreenCombat;
    [SerializeField] RewardScreen rewardScreen;
    public RewardScreenCardSelection rewardScreenCardSelection;
    public int draftAmount = 0;
    public System.Action rewardCallback;
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
    public void CreateRewards(RewardType[] rewards, Transform parent)
    {
        Debug.LogWarning("körs denna??? säg till om du ser");
        foreach (RewardType reward in rewards)
        {
            Reward newReward = Instantiate(WorldSystem.instance.rewardManager.rewardPrefab, parent).GetComponent<Reward>();
            newReward.SetupReward(reward);
        }
    }

    public void GetReward(RewardType type, string[] value = null, bool fromEvent = false)
    {
        rewardScreen.GetReward(type, value, fromEvent);
    }

    public void CopyReward(Reward aReward)
    {
        rewardScreen.CopyReward(aReward);
    }

    public void OpenRewardScreen()
    {
        rewardScreen.canvas.gameObject.SetActive(true);
    }

    public void ClearRewardScreen()
    {
        if (rewardScreen.reward != null)
            Destroy(rewardScreen.reward.gameObject);
            
        WorldStateSystem.TriggerClear();
        rewardScreen.canvas.gameObject.SetActive(false);
    }

    public void Unsubscribe()
    {
        EventManager.OnEnemyKilledEvent -= EnemyKilled;
    }
}
