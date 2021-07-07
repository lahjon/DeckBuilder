
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Reward : MonoBehaviour, IToolTipable
{
    public ItemData itemData;
    public Transform tooltipAnchor;
    public TMP_Text rewardText;
    public Image image; 
    public RewardType rewardType;
    bool reset;
    Action callback;
    public void OnClick()
    {
        CollectCombatReward();
        GetComponent<ToolTipScanner>().ExitAction();
        Destroy(gameObject);
        WorldSystem.instance.rewardManager.rewardScreenCombat.rewardCount--;

        if(reset)
            WorldSystem.instance.rewardManager.rewardScreenCombat.ResetReward();
    }
    public void SetupReward(RewardType aRewardType, string[] aValue = null)
    {
        rewardType = aRewardType;
        GetComponent<Button>().onClick.AddListener(OnClick);
        switch (rewardType)
        {
            case RewardType.Gold:
                RewardGold(aValue);
                break;
            case RewardType.Shard:
                RewardShard(aValue);
                break;
            case RewardType.Artifact:
                RewardArtifact(aValue);
                break;
            case RewardType.Card:
                RewardCard(aValue);
                break;
            case RewardType.Item:
                RewardItem(aValue);
                break;
            case RewardType.Heal:
                RewardHeal(aValue);
                break;
            default:
                break;
        }
    }

    #region RewardTypes
    public void RewardGold(string[] value)
    {
        float startRange = 15;
        float endRange = 25;

        int amount = (value != null && value.Count() == 0) ? (int)UnityEngine.Random.Range(startRange, endRange) : int.Parse(value[0]);
        rewardText.text = string.Format("Gold: " + amount.ToString());
        image.sprite = WorldSystem.instance.rewardManager.icons[0];
        reset = true;
        
        callback = () => WorldSystem.instance.characterManager.gold += amount;
    }
    public void RewardShard(string[] value)
    {
        float startRange = 3;
        float endRange = 5;
        int amount = (value != null && value.Count() == 0) ? (int)UnityEngine.Random.Range(startRange, endRange) : int.Parse(value[0]);
        rewardText.text = string.Format("Shard: " + amount.ToString());
        image.sprite = WorldSystem.instance.rewardManager.icons[1];
        reset = true;

        callback = () => WorldSystem.instance.characterManager.shard += amount;
    }
    public void RewardItem(string[] value)
    {
        itemData = (value != null && value.Count() == 0) ? WorldSystem.instance.useItemManager.GetItemData() : WorldSystem.instance.useItemManager.GetItemData(value[0]);

        if (itemData == null)
        {
            Debug.Log("No artifact found!");
            return;
        }

        rewardText.text = itemData.itemName;
        image.sprite = itemData.artwork;
        reset = true;

        callback = () => WorldSystem.instance.useItemManager.AddItem(itemData.name);
    }
    public void RewardArtifact(string[] value)
    {
        itemData = (value != null && value.Count() == 0) ? WorldSystem.instance.artifactManager.GetRandomAvailableArtifact() : WorldSystem.instance.artifactManager.GetSpecficArtifact(value[0]);

        if (itemData == null)
        {
            Debug.Log("No artifact found!");
            return;
        }

        rewardText.text = itemData.itemName;
        image.sprite = itemData.artwork;
        reset = true;

        callback = () => WorldSystem.instance.artifactManager.AddArtifact(itemData.name);
    }
    public void RewardHeal(string[] value)
    {
        callback = () => Debug.Log("No Reward Implemented!");
    }
    public void RewardCard(string[] value)
    {
        rewardText.text = "Card";
        image.sprite = WorldSystem.instance.rewardManager.icons[2];
        List<CardData> cardDatas = new List<CardData>();

        if (value != null && value.Count() > 0)
        {
            CardFilter cardFilter = new CardFilter();
            foreach (string item in value)
            {
                if (item.Contains("name"))
                {
                    cardDatas.Clear();
                    string[] cardNames = item.Split('=')[1].Split(';');
                    foreach (string card in cardNames)
                    {
                        cardFilter.name = card;
                        cardDatas.Add(DatabaseSystem.instance.GetRandomCard(cardFilter));
                    }
                    break;
                }
                else
                {
                    if (item.Contains("rarity"))
                    {
                        string[] rarity = item.Split('=')[1].Split(';');
                        float x = float.Parse(rarity[0]);
                        float y = float.Parse(rarity[1]);
                        cardFilter.rarity = Helpers.DrawRarity(x, y);
                    }
                    else if (item.Contains("cost"))
                    {
                        cardFilter.costArr = Array.ConvertAll(item.Split('=')[1].Split(';'), x => int.Parse(x));
                    }
                    else if (item.Contains("notClassType"))
                    {
                        cardFilter.notClassTypeArr = Array.ConvertAll(item.Split('=')[1].Split(';'), x => (CardClassType)System.Enum.Parse(typeof(CardClassType), x));
                    }
                    else if (item.Contains("classType"))
                        cardFilter.classTypeArr = Array.ConvertAll(item.Split('=')[1].Split(';'), x => (CardClassType)System.Enum.Parse(typeof(CardClassType), x));
                }
            }
            if (cardDatas.Count == 0) 
            {
                if (cardFilter.classTypeArr == null)
                    cardFilter.classType = (CardClassType)WorldSystem.instance.characterManager.selectedCharacterClassType;
                for (int i = 0; i < 3; i++)
                {
                    if (DatabaseSystem.instance.GetRandomCard(cardFilter) is CardData data && data != null)
                        cardDatas.Add(data);
                }
            }

        }
        callback = () => WorldSystem.instance.rewardManager.rewardScreenCombat.rewardScreenCard.GetComponent<RewardScreenCardSelection>().SetupRewards(cardDatas);
    }
    #endregion

    public void CollectCombatReward()
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
        if((int)rewardType >= 10)
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

[System.Serializable] public struct RewardStruct
{
    public RewardType type;
    public string[] value;
}
