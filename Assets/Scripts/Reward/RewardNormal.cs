
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class RewardNormal : Reward
{
    public RewardNormalType rewardType;
    public override void AddReward()
    {
        transform.SetParent(WorldSystem.instance.rewardManager.rewardParent);
        WorldSystem.instance.rewardManager.uncollectedReward.Add(this);
    }
    public override void RemoveReward()
    {
        WorldSystem.instance.rewardManager.uncollectedReward.Remove(this);
        Destroy(gameObject);
        WorldSystem.instance.rewardManager.CollectRewards();
    }
    public void SetupReward(RewardNormalType aRewardType, string aValue = null)
    {
        rewardType = aRewardType;
        value = aValue;
        GetComponent<Button>().onClick.AddListener(OnClick);
        switch (rewardType)
        {
            case RewardNormalType.Perk:
                RewardPerk(aValue);
                break;
            case RewardNormalType.Shard:
                RewardShard(aValue);
                break;
            case RewardNormalType.UnlockCard:
                RewardUnlockCard(aValue);
                break;
            case RewardNormalType.Ability:
                RewardAbility(aValue);
                break;

            default:
                break;
        }
    }

    #region RewardTypes

    public void RewardPerk(string value)
    {
        itemData = (value != null && value.Count() > 0) ? WorldSystem.instance.menuManager.menuCharacter.GetPerkById(Int32.Parse(value)) : null;
        if (itemData == null)
        {
            Debug.LogWarning("No Perk found!");
            return;
        }

        rewardText.text = "Perk";
        image.sprite = itemData.artwork;

        callback = () => WorldSystem.instance.menuManager.menuCharacter.UnlockPerk(itemData.itemId);
    }
    public void RewardShard(string value)
    {
        float startRange = 3;
        float endRange = 5;
        int amount = (value != null && value.Count() > 0) ? int.Parse(value) : (int)UnityEngine.Random.Range(startRange, endRange);
        rewardText.text = string.Format("Shard: {0}", amount.ToString());
        image.sprite = WorldSystem.instance.rewardManager.icons[1];
        rewardAmount = amount;

        callback = () => WorldSystem.instance.characterManager.characterCurrency.shard += amount;
    }
    public void RewardAbility(string value)
    {
        itemData = (value != null && value.Count() > 0) ? WorldSystem.instance.abilityManager.GetAbilityDataById(Int32.Parse(value)) : WorldSystem.instance.abilityManager.GetAbilityDataById();

        if (itemData == null)
        {
            Debug.LogWarning("No Item found!");
            return;
        }
        // if (WorldSystem.instance.abilityManager.currentAbilities.Select(x => x.id).Contains(itemData.itemId))
        // {
        //     Debug.LogWarning("Already have that item!");
        //     return;
        // }

        rewardText.text = itemData.itemName;
        image.sprite = itemData.artwork;

        callback = () => WorldSystem.instance.abilityManager.EquipAbility(itemData.itemId);
    }

    public void RewardUnlockCard(string value)
    {
        image.sprite = WorldSystem.instance.rewardManager.icons[2];
        callback = () => {
            if (value?.Any() == true && GetCardData(value) is List<CardData> datas)
            {
                datas.ForEach(x => WorldSystem.instance.townManager.scribe.UnlockCard(x));
            }
            else
            {
                Debug.LogWarning("No value for unlock card reward!");
            }
        };
    }
    #endregion
    public override (List<string> tips, Vector3 worldPosition, float offset) GetTipInfo()
    {
        Vector3 pos;
        string desc;
        switch (rewardType)
        {
            case RewardNormalType.Ability:
                pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
                desc = string.Format("<b>{0}{1}/<b>", itemData.itemName, itemData.description);
                return (new List<string>{desc} , pos, width);
            case RewardNormalType.UnlockCard:
                pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
                desc = "<b>Unlock a new card</b>";
                return (new List<string>{desc} , pos, width);
            
            default:
                return (new List<string>{} , Vector3.zero, width);
        }
    }
}