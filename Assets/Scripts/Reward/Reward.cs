
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Reward : MonoBehaviour, IToolTipable
{
    public ItemData itemData;
    public Transform tooltipAnchor;
    public TMP_Text rewardText;
    public Image image; 
    public RewardType rewardType;
    string value;
    bool reset;
    Action callback;
    public void OnClick()
    {
        CollectCombatReward();
        WorldSystem.instance.toolTipManager.DisableTips();
        DestroyImmediate(gameObject);

        if(reset)
            WorldSystem.instance.rewardManager.rewardScreenCombat.ResetReward();
    }
    public void SetupReward(string aValue = "")
    {
        switch (rewardType)
        {
            case RewardType.Gold:
                RewardGold(value);
                break;
            case RewardType.Shard:
                RewardShard(value);
                break;
            case RewardType.Artifact:
                RewardArtifact(value);
                break;
            case RewardType.Card:
                RewardCard(value);
                break;
            case RewardType.Item:
                RewardItem(value);
                break;
            case RewardType.Heal:
                RewardHeal(value);
                break;
            default:
                break;
        }
    }

    #region RewardTypes
    public void RewardGold(string value)
    {
        //CombatSystem.instance.encounterData.type
        float startRange = 15;
        float endRange = 25;

        int amount = (int)UnityEngine.Random.Range(startRange, endRange);
        rewardText.text = string.Format("Gold: " + amount.ToString());
        image.sprite = WorldSystem.instance.rewardManager.icons[0];
        reset = true;
        
        callback = () => WorldSystem.instance.characterManager.gold += amount;
    }
    public void RewardShard(string value)
    {
        float startRange = 3;
        float endRange = 5;

        int amount = (int)UnityEngine.Random.Range(startRange, endRange);
        rewardText.text = string.Format("Shard: " + amount.ToString());
        image.sprite = WorldSystem.instance.rewardManager.icons[1];
        reset = true;

        callback = () => WorldSystem.instance.characterManager.shard += amount;
    }
    public void RewardArtifact(string value)
    {
        if (string.IsNullOrEmpty(value))
            itemData = WorldSystem.instance.artifactManager.GetRandomAvailableArtifact();
        else
            itemData = WorldSystem.instance.artifactManager.GetSpecficArtifact(value);

        rewardText.text = itemData.itemName;
        image.sprite = itemData.artwork;
        reset = true;

        callback = () => WorldSystem.instance.artifactManager.AddArtifact(itemData.itemName);
    }
    public void RewardItem(string value)
    {
        callback = () => Debug.Log("No Reward Implemented!");
    }
    public void RewardHeal(string value)
    {
        callback = () => Debug.Log("No Reward Implemented!");
    }
    public void RewardCard(string value)
    {
        rewardText.text = "Card";
        image.sprite = WorldSystem.instance.rewardManager.icons[2];
        callback = () => WorldSystem.instance.rewardManager.rewardScreenCombat.rewardScreenCard.GetComponent<RewardScreenCardSelection>().SetupRewards();
    }
    #endregion

    void CollectCombatReward()
    {
        if (callback != null)
        {
            callback.Invoke();
            callback = null;
        }
    }

    public (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        Vector3 pos;
        string desc;
        if((int)rewardType > 10)
        {
            // x > 10 has itemdata and can extract the description
            pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
            desc = string.Format("<b>" + itemData.itemName + "</b>\n" + itemData.description);
            return (new List<string>{desc} , pos);
        }
        else
            return (new List<string>{} , Vector3.zero);
        
    }
}
