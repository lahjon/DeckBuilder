using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class RewardManager : Manager
{
    public GameObject rewardNormalPrefab;
    public Sprite[] icons;
    public RewardScreen rewardScreen;
    public RewardScreenCardSelection rewardScreenCardSelection;
    public List<RewardNormal> uncollectedReward;
    public Transform rewardParent;
    protected override void Awake()
    {
        base.Awake();
        world.rewardManager = this;
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
    public RewardNormal CreateRewardNormal(RewardNormalType type, string value = null, Transform parent = null, bool addReward = true)
    {
        if (parent == null) parent = rewardParent;

        RewardNormal reward = Instantiate(rewardNormalPrefab, parent).GetComponent<RewardNormal>();
        reward.gameObject.SetActive(false);
        reward.SetupReward(type, value);
        if (addReward) reward.AddReward();
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
}


[System.Serializable] public struct RewardNormalStruct
{
    public RewardNormalType type;
    public string value;
}