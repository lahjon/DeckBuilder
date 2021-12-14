using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class CombatRewardManager : Manager
{
    
    public RewardScreenCombat rewardScreenCombat;
    public GameObject rewardCombatPrefab;
    public Transform rewardParent;
    public RewardScreenCardSelection rewardScreenCardSelection;
    public List<RewardCombat> uncollectedReward;
    public RewardCombatStruct[] combatRewardNormal;
    public RewardCombatStruct[] combatRewardElite;
    public RewardCombatStruct[] combatRewardBoss;
    RewardScreen rewardScreen {get => WorldSystem.instance.rewardManager.rewardScreen;}
    protected override void Awake()
    {
        base.Awake();
        world.combatRewardManager = this;
    }
    public void OpenCombatRewardScreen()
    {
        rewardScreenCombat.SetupRewards();
    }

    public RewardCombat CreateRewardCombat(RewardCombatType type, string value = null)
    {
        RewardCombat reward = Instantiate(rewardCombatPrefab, rewardParent).GetComponent<RewardCombat>();
        reward.gameObject.SetActive(false);
        reward.SetupReward(type, value);
        reward.AddReward();
        Debug.Log("AddedReward");
        return reward;
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


    public void OpenDraftMode()
    {
        rewardScreenCardSelection.SetupRewards();
    }
 
}

[System.Serializable] public struct RewardCombatStruct
{
    public RewardCombatType type;
    public string value;
}
