
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class RewardCombat : Reward
{
    public RewardCombatType rewardType;
    bool reset;
    public override void AddReward()
    {
        WorldSystem.instance.combatRewardManager.uncollectedReward.Add(this);
    }
    public override void RemoveReward()
    {
        WorldSystem.instance.combatRewardManager.uncollectedReward.Remove(this);
        Destroy(gameObject);
        WorldSystem.instance.rewardManager.CollectRewards();
    }
    public override void OnClick()
    {
        base.OnClick();
        WorldSystem.instance.combatRewardManager.rewardScreenCombat.rewardCount--;

        if(reset)
            WorldSystem.instance.combatRewardManager.rewardScreenCombat.ResetReward();
    }
    public void SetupReward(RewardCombatType aRewardType, string aValue = null)
    {
        rewardType = aRewardType;
        value = aValue;
        GetComponent<Button>().onClick.AddListener(OnClick);
        switch (rewardType)
        {
            case RewardCombatType.Gold:
                RewardGold(aValue);
                break;
            case RewardCombatType.Artifact:
                RewardArtifact(aValue);
                break;
            case RewardCombatType.Card:
                RewardCard(aValue);
                break;
            case RewardCombatType.Heal:
                RewardHeal(aValue);
                break;
            case RewardCombatType.Shard:
                RewardShard(aValue);
                break;
            case RewardCombatType.Ember:
                RewardEmber(aValue);
                break;

            default:
                break;
        }
    }

    #region RewardTypes
    public void RewardGold(string value)
    {
        float startRange = 15;
        float endRange = 25;

        int amount = (value != null && value.Count() > 0) ? int.Parse(value) : (int)UnityEngine.Random.Range(startRange, endRange);
        rewardText.text = string.Format("Gold: {0}", amount.ToString());
        image.sprite = WorldSystem.instance.rewardManager.icons[0];
        reset = true;
        rewardAmount = amount;
        
        callback = () => WorldSystem.instance.characterManager.characterCurrency.gold += amount;
    }
    public void RewardShard(string value)
    {
        float startRange = 3;
        float endRange = 5;
        int amount = (value != null && value.Count() > 0) ? int.Parse(value) : (int)UnityEngine.Random.Range(startRange, endRange);
        rewardText.text = string.Format("Shard: {0}", amount.ToString());
        image.sprite = WorldSystem.instance.rewardManager.icons[1];
        rewardAmount = amount;
        reset = true;

        callback = () => WorldSystem.instance.characterManager.characterCurrency.shard += amount;
    }

    public void RewardEmber(string value)
    {
        int amount = (value != null && value.Count() > 0) ? int.Parse(value) : 1;
        rewardText.text = string.Format("Ember: {0}", amount.ToString());
        image.sprite = WorldSystem.instance.rewardManager.icons[4];
        rewardAmount = amount;
        reset = true;

        callback = () => WorldSystem.instance.characterManager.characterCurrency.ember += amount;
    }

    public void RewardArtifact(string value)
    {
        itemData = (value != null && value.Count() > 0) ? WorldSystem.instance.artifactManager.GetSpecficArtifact(Int32.Parse(value)) : WorldSystem.instance.artifactManager.GetRandomAvailableArtifact();

        if (itemData == null)
        {
            Debug.LogWarning("No artifact found!");
            return;
        }

        rewardText.text = itemData.itemName;
        image.sprite = itemData.artwork;
        reset = true;

        callback = () => WorldSystem.instance.artifactManager.AddArtifact(itemData.itemId);
    }
    public void RewardHeal(string value)
    {
        callback = () => Debug.LogError("No Reward Implemented!");
    }
    public void RewardCard(string value)
    {
        rewardText.text = "Card";
        image.sprite = WorldSystem.instance.rewardManager.icons[2];

        callback = () => WorldSystem.instance.rewardManager.rewardScreenCardSelection.SetupRewards(GetCardData(value));
    }
    #endregion

    public override (List<string> tips, Vector3 worldPosition, float offset) GetTipInfo()
    {
        Vector3 pos;
        string desc;
        switch (rewardType)
        {
            case RewardCombatType.Artifact:
                pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
                desc = string.Format("<b>{0}{1}/<b>", itemData.itemName, itemData.description);
                return (new List<string>{desc} , pos, width);
            case RewardCombatType.Gold:
                pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
                desc = string.Format("<b>{0} Gold</b>", rewardAmount.ToString());
                return (new List<string>{desc} , pos, width);
            
            default:
                return (new List<string>{} , Vector3.zero, width);
        }
    }
}