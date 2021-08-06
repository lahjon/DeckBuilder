using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardScreen : MonoBehaviour
{
    public Reward rewardPrefab;
    public GameObject content;
    public Transform anchor;
    public Canvas canvas;
    public Reward reward;
    public CardDisplay[] displayCards;
    public GameObject cardPanel;

    public void GetReward(RewardType rewardType, string[] value = null)
    {
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
                            cardDisplay.clickCallback = () => {
                                WorldSystem.instance.characterManager.AddCardDataToDeck(cardDisplay.cardData);
                                WorldSystem.instance.deckDisplayManager.StartCoroutine(cardDisplay.AnimateCardToDeck());
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
                reward.GetComponent<Button>().onClick.RemoveAllListeners();

                Helpers.DelayForSeconds(.3f, () => {
                    reward.GetComponent<Button>().onClick.AddListener(() => {
                        reward.CollectCombatReward();
                        reward.GetComponent<ToolTipScanner>().ExitAction();
                        WorldSystem.instance.rewardManager.ClearRewardScreen();
                    });
                    content.GetComponent<Button>().onClick.AddListener(() => {
                        reward.CollectCombatReward();
                        reward.GetComponent<ToolTipScanner>().ExitAction();
                        WorldSystem.instance.rewardManager.ClearRewardScreen();
                    });
                });
                
                WorldStateSystem.SetInRewardScreen();
                break;
        }
    }
}
