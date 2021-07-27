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
                        displayCards[i].cardData = cardDatas[i];
                        displayCards[i].BindCardData();
                        displayCards[i].BindCardVisualData();
                        displayCards[i].gameObject.SetActive(true);
                        CardDisplay cardDisplay = displayCards[i];
                        displayCards[i].clickCallback = () => {
                            WorldSystem.instance.characterManager.AddCardDataToDeck(cardDisplay.cardData);
                            WorldSystem.instance.deckDisplayManager.StartCoroutine(cardDisplay.AnimateCardToDeck());
                            cardPanel.SetActive(false);
                            WorldSystem.instance.rewardManager.ClearRewardScreen();
                        };
                    }
                    // content.GetComponent<Button>().onClick.AddListener(() => {
                    //     cardPanel.SetActive(false);
                    //     WorldSystem.instance.toolTipManager.DisableTips();
                    //     WorldSystem.instance.rewardManager.ClearRewardScreen();
                    // });
                    WorldStateSystem.SetInRewardScreen();
                }

                break;
            default:
                reward = Instantiate(rewardPrefab, anchor).GetComponent<Reward>();   
                reward.SetupReward(rewardType, value);
                Destroy(reward.rewardText.gameObject);
                reward.transform.localScale = Vector3.one * 2;
                reward.GetComponent<Button>().onClick.RemoveAllListeners();
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
                WorldStateSystem.SetInRewardScreen();
                break;
        }
    }
}
