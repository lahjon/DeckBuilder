
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Globalization;

public class Reward : MonoBehaviour, IToolTipable
{
    public ItemData itemData;
    public Transform tooltipAnchor;
    public TMP_Text rewardText;
    public Image image; 
    public RewardType rewardType;
    public TMP_Text rewardCountText;
    bool reset;
    public string[] value;
    public Action callback;
    public int rewardAmount;
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
        value = aValue;
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
            case RewardType.UnlockCard:
                RewardUnlockCard(aValue);
                break;
            default:
                break;
        }
    }

    public void SetWorldReward()
    {
        rewardCountText.text = rewardAmount.ToString();
        rewardCountText.gameObject.SetActive(true);
        rewardText.gameObject.SetActive(false);
    }

    public void AddReward()
    {
        WorldSystem.instance.rewardManager.uncollectedReward.Add(this);
        transform.SetParent(WorldSystem.instance.rewardManager.rewardParent);
    }
    public void RemoveReward()
    {
        WorldSystem.instance.rewardManager.uncollectedReward.Remove(this);
        Destroy(gameObject);
        WorldSystem.instance.rewardManager.CollectRewards();
    }

    #region RewardTypes
    public void RewardGold(string[] value)
    {
        float startRange = 15;
        float endRange = 25;

        int amount = (value != null && value.Count() > 0) ? int.Parse(value[0]) : (int)UnityEngine.Random.Range(startRange, endRange);
        rewardText.text = string.Format("Gold: " + amount.ToString());
        image.sprite = WorldSystem.instance.rewardManager.icons[0];
        reset = true;
        rewardAmount = amount;
        
        callback = () => WorldSystem.instance.characterManager.gold += amount;
    }
    public void RewardShard(string[] value)
    {
        float startRange = 3;
        float endRange = 5;
        int amount = (value != null && value.Count() > 0) ? int.Parse(value[0]) : (int)UnityEngine.Random.Range(startRange, endRange);
        rewardText.text = string.Format("Shard: " + amount.ToString());
        image.sprite = WorldSystem.instance.rewardManager.icons[1];
        reset = true;
        rewardAmount = amount;

        callback = () => WorldSystem.instance.characterManager.shard += amount;
    }
    public void RewardItem(string[] value)
    {
        itemData = (value != null && value.Count() > 0) ? WorldSystem.instance.useItemManager.GetItemData(value[0]) : WorldSystem.instance.useItemManager.GetItemData();

        if (itemData == null)
        {
            Debug.LogWarning("No Item found!");
            return;
        }

        rewardText.text = itemData.itemName;
        image.sprite = itemData.artwork;
        reset = true;

        callback = () => WorldSystem.instance.useItemManager.AddItem(itemData.name);
    }
    public void RewardArtifact(string[] value)
    {
        itemData = (value != null && value.Count() > 0) ? WorldSystem.instance.artifactManager.GetSpecficArtifact(value[0]) : WorldSystem.instance.artifactManager.GetRandomAvailableArtifact();

        if (itemData == null)
        {
            Debug.LogWarning("No artifact found!");
            return;
        }

        rewardText.text = itemData.itemName;
        image.sprite = itemData.artwork;
        reset = true;

        callback = () => WorldSystem.instance.artifactManager.AddArtifact(itemData.name);
    }
    public void RewardHeal(string[] value)
    {
        callback = () => Debug.LogError("No Reward Implemented!");
    }
    public void RewardCard(string[] value)
    {
        rewardText.text = "Card";
        image.sprite = WorldSystem.instance.rewardManager.icons[2];

        callback = () => WorldSystem.instance.rewardManager.rewardScreenCombat.rewardScreenCard.GetComponent<RewardScreenCardSelection>().SetupRewards(GetCardData(value));
    }

    public void RewardUnlockCard(string[] value)
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

    public static List<CardData> GetCardData(string[] value)
    {
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
                        float x = float.Parse(rarity[0], CultureInfo.InvariantCulture.NumberFormat);
                        float y = float.Parse(rarity[1], CultureInfo.InvariantCulture.NumberFormat);
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
        return cardDatas;
    }

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
        switch (rewardType)
        {
            case RewardType.Item:
            case RewardType.Artifact:
                pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
                desc = string.Format("<b>" + itemData.itemName + "</b>\n" + itemData.description);
                return (new List<string>{desc} , pos);
            case RewardType.UnlockCard:
                pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
                desc = string.Format("<b>" + "Unlock a new card" + "</b>");
                return (new List<string>{desc} , pos);
            case RewardType.Gold:
                pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
                desc = string.Format("<b>" + rewardAmount.ToString() + " Gold" + "</b>");
                return (new List<string>{desc} , pos);
            case RewardType.Shard:
                pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
                desc = string.Format("<b>" + rewardAmount.ToString() + " Shards" + "</b>");
                return (new List<string>{desc} , pos);
            
            default:
                return (new List<string>{} , Vector3.zero);
        }
    }
}

[System.Serializable] public struct RewardStruct
{
    public RewardType type;
    public string[] value;
}
