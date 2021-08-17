using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardScreen : MonoBehaviour
{
    public Reward rewardPrefab;
    public GameObject content;
    public Transform anchor;
    public Canvas canvas;
    public Reward reward;
    public CardDisplay[] displayCards;
    public GameObject cardPanel;
    public TMP_Text headerText;


    public void GetReward(RewardType rewardType, string[] value = null, bool fromEvent = false) // creates a new reward and opens the reward screen
    {
        SetHeader(rewardType);
        Debug.Log("Dick");
        content.GetComponent<Button>().onClick.RemoveAllListeners();
        // make sure to delay all onClick to make sure player dont double click and remove reward screen instantly
        switch (rewardType)
        {
            case RewardType.Card:
                cardPanel.SetActive(true);
                foreach (var item in displayCards)
                {
                    item.gameObject.SetActive(false);
                }
                if (Reward.GetCardData(value) is List<CardData> cardDatas)
                {
                    for (int i = 0; i < cardDatas.Count; i++)
                    {
                        if (i >= displayCards.Length) break;

                        CardDisplay cardDisplay = displayCards[i];

                        cardDisplay.clickCallback = null;

                        cardDisplay.cardData = cardDatas[i];
                        cardDisplay.BindCardData();
                        cardDisplay.BindCardVisualData();
                        cardDisplay.gameObject.SetActive(true);

                        Helpers.DelayForSeconds(.3f, () => {
                            Debug.Log("1");
                            cardDisplay.clickCallback = () => {
                                WorldSystem.instance.characterManager.AddCardDataToDeck(cardDisplay.cardData);
                                WorldSystem.instance.deckDisplayManager.StartCoroutine(cardDisplay.AnimateCardToDeck());
                                cardPanel.SetActive(false);
                                WorldStateSystem.SetInEvent(false);
                                WorldSystem.instance.rewardManager.ClearRewardScreen();
                            };
                        });
                    }
                    WorldStateSystem.SetInRewardScreen();
                }
                break;

            case RewardType.UnlockCard:
                cardPanel.SetActive(true);
                foreach (var item in displayCards)
                {
                    item.gameObject.SetActive(false);
                }
                if (Reward.GetCardData(value) is List<CardData> cardUnlockDatas)
                {
                    for (int i = 0; i < cardUnlockDatas.Count; i++)
                    {
                        if (i >= displayCards.Length) break;

                        CardDisplay cardDisplay = displayCards[i];

                        cardDisplay.clickCallback = null;

                        cardDisplay.cardData = cardUnlockDatas[i];
                        cardDisplay.BindCardData();
                        cardDisplay.BindCardVisualData();
                        cardDisplay.gameObject.SetActive(true);

                        Helpers.DelayForSeconds(.3f, () => {
                            cardDisplay.clickCallback = () => {
                                WorldSystem.instance.townManager.scribe.UnlockCard(cardDisplay.cardData);
                                //WorldSystem.instance.deckDisplayManager.StartCoroutine(cardDisplay.AnimateCardToDeck());
                                cardPanel.SetActive(false);
                                WorldSystem.instance.rewardManager.ClearRewardScreen();
                            };
                        });
                    }
                    WorldStateSystem.SetInRewardScreen();
                }
            break;

            default:
                reward = Instantiate(rewardPrefab, anchor).GetComponent<Reward>();   
                reward.SetupReward(rewardType, value);
                Destroy(reward.rewardText.gameObject);
                reward.transform.localScale = Vector3.one * 2;
                CreateReward(reward);
                break;
        }
    }

    public void CopyReward(Reward aReward) // takes an existing reward and copies it over to the reward screen
    {
        SetHeader(aReward.rewardType);
        switch (aReward.rewardType)
        {
            case RewardType.UnlockCard:
                cardPanel.SetActive(true);
                foreach (var item in displayCards)
                {
                    item.gameObject.SetActive(false);
                }
                if (Reward.GetCardData(aReward.value) is List<CardData> cardDatas)
                {
                    for (int i = 0; i < cardDatas.Count; i++)
                    {
                        if (i >= displayCards.Length) break;

                        CardDisplay cardDisplay = displayCards[i];

                        cardDisplay.clickCallback = null;

                        cardDisplay.cardData = cardDatas[i];
                        cardDisplay.BindCardData();
                        cardDisplay.BindCardVisualData();
                        cardDisplay.gameObject.SetActive(true);

                        Helpers.DelayForSeconds(.3f, () => {
                            cardDisplay.clickCallback = () => {
                                WorldSystem.instance.townManager.scribe.UnlockCard(cardDisplay.cardData);
                                //WorldSystem.instance.deckDisplayManager.StartCoroutine(cardDisplay.AnimateCardToDeck());
                                cardPanel.SetActive(false);
                                WorldSystem.instance.rewardManager.ClearRewardScreen();
                            };
                        });
                    }
                    WorldStateSystem.SetInRewardScreen();
                }
            
                break;
            default:
                reward = Instantiate(aReward, anchor).GetComponent<Reward>();   
                reward.callback = aReward.callback;
                reward.gameObject.SetActive(true);
                CreateReward(reward);
                break;
        }
    }

    void SetHeader(RewardType aRewardType)
    {
        switch (aRewardType)
        {
            case RewardType.UnlockCard:
                headerText.text = "New card unlocked!";
                break;
            case RewardType.Card:
                headerText.text = "New card!";
                break;
            default:
                headerText.text = "New reward!";
                break;
        }
    }

    void CreateReward(Reward reward)
    {
        reward.transform.localScale = Vector3.one * 2;
        reward.GetComponent<Button>().onClick.RemoveAllListeners();

        void RewardCallback()
        {
            reward.CollectCombatReward();
            reward.GetComponent<ToolTipScanner>().ExitAction();
            WorldStateSystem.SetInEvent(false);
            WorldSystem.instance.rewardManager.ClearRewardScreen();
        }

        Helpers.DelayForSeconds(.3f, () => {
            reward.GetComponent<Button>().onClick.AddListener(() => RewardCallback());
            content.GetComponent<Button>().onClick.AddListener(() => RewardCallback());
        });
        
        WorldStateSystem.SetInRewardScreen();
    }
}
