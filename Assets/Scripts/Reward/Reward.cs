
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Globalization;

public abstract class Reward : MonoBehaviour, IToolTipable
{
    public ItemData itemData;
    public Transform tooltipAnchor;
    public TMP_Text rewardText;
    public Image image; 
    public TMP_Text rewardCountText;
    public string value;
    public Action callback;
    public int rewardAmount;
    public virtual void OnClick()
    {
        CollectCombatReward();
        GetComponent<ToolTipScanner>().ExitAction();
        Destroy(gameObject);
    }
    public void SetWorldReward()
    {
        rewardCountText.text = rewardAmount.ToString();
        rewardCountText.gameObject.SetActive(true);
        rewardText.gameObject.SetActive(false);
    }
    public abstract void AddReward();
    public abstract void RemoveReward();

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

    public abstract (List<string> tips, Vector3 worldPosition) GetTipInfo();
}
