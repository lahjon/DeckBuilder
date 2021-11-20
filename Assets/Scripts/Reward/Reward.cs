
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
    public string value;
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
    public void SetupReward(RewardType aRewardType, string aValue = null)
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
            case RewardType.Perk:
                RewardPerk(aValue);
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
        reset = true;
        rewardAmount = amount;

        callback = () => WorldSystem.instance.characterManager.characterCurrency.shard += amount;
    }
    public void RewardItem(string value)
    {
        itemData = (value != null && value.Count() > 0) ? WorldSystem.instance.abilityManager.GetItemData(Int32.Parse(value)) : WorldSystem.instance.abilityManager.GetItemData();

        if (itemData == null)
        {
            Debug.LogWarning("No Item found!");
            return;
        }
        if (WorldSystem.instance.abilityManager.currentAbilities.Select(x => x.id).Contains(itemData.itemId))
        {
            Debug.LogWarning("Already have that item!");
            return;
        }

        rewardText.text = itemData.itemName;
        image.sprite = itemData.artwork;
        reset = true;

        callback = () => WorldSystem.instance.abilityManager.AddItem(itemData.itemId);
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

    public static List<CardData> GetCardData(string value)
    {
        List<CardData> cardDatas = new List<CardData>();

        if (value != null && value.Count() > 0)
        {
            CardFilter cardFilter = new CardFilter();

            if (value.Contains("name"))
            {
                cardDatas.Clear();
                string[] cardNames = value.Split('=')[1].Split(';');
                foreach (string card in cardNames)
                {
                    cardFilter.name = card;
                    cardDatas.Add(DatabaseSystem.instance.GetRandomCard(cardFilter));
                }
            }
            else
            {
                if (value.Contains("rarity"))
                {
                    string[] rarity = value.Split('=')[1].Split(';');
                    float x = float.Parse(rarity[0], CultureInfo.InvariantCulture.NumberFormat);
                    float y = float.Parse(rarity[1], CultureInfo.InvariantCulture.NumberFormat);
                    cardFilter.rarity = Helpers.DrawRarity(x, y);
                }
                else if (value.Contains("cost"))
                {
                    cardFilter.costArr = Array.ConvertAll(value.Split('=')[1].Split(';'), x => int.Parse(x));
                }
                else if (value.Contains("notClassType"))
                {
                    cardFilter.notClassTypeArr = Array.ConvertAll(value.Split('=')[1].Split(';'), x => (CardClassType)System.Enum.Parse(typeof(CardClassType), x));
                }
                else if (value.Contains("classType"))
                    cardFilter.classTypeArr = Array.ConvertAll(value.Split('=')[1].Split(';'), x => (CardClassType)System.Enum.Parse(typeof(CardClassType), x));
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
                desc = string.Format("<b>{0}{1}/<b>", itemData.itemName, itemData.description);
                return (new List<string>{desc} , pos);
            case RewardType.UnlockCard:
                pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
                desc = "<b>Unlock a new card</b>";
                return (new List<string>{desc} , pos);
            case RewardType.Gold:
                pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
                desc = string.Format("<b>{0} Gold</b>", rewardAmount.ToString());
                return (new List<string>{desc} , pos);
            case RewardType.Shard:
                pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
                desc = string.Format("<b>{0} Shards</b>", rewardAmount.ToString());
                return (new List<string>{desc} , pos);
            
            default:
                return (new List<string>{} , Vector3.zero);
        }
    }
}

[System.Serializable] public struct RewardStruct
{
    public RewardType type;
    public string value;
}
