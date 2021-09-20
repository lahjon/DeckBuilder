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
    public Reward currentReward;
    public CardDisplay[] displayCards;
    public GameObject cardPanel;
    public TMP_Text headerText;

    public void CollectReward(Reward aReward) // takes an existing reward and copies it over to the reward screen
    {
        SetHeader(aReward.rewardType);
        currentReward = aReward;
        switch (aReward.rewardType)
        {
            case RewardType.Card:
                CreateCardReward(aReward);
                break;

            case RewardType.UnlockCard:
                CreateUnlockCardReward(aReward);
                break;

            default:
                currentReward.transform.SetParent(anchor);
                currentReward.gameObject.SetActive(true);
                AddRewardToScreen(currentReward);
                break;
        }
    }

    void CreateCardReward(Reward aReward)
    {
        if (Reward.GetCardData(aReward.value) is List<CardData> cardDatas)
        {
            cardPanel.SetActive(true);
            foreach (var item in displayCards)
            {
                item.gameObject.SetActive(false);
            }
            for (int i = 0; i < cardDatas.Count; i++)
            {
                if (i >= displayCards.Length) break;

                CardDisplay cardDisplay = displayCards[i];

                cardDisplay.OnClick = null;

                cardDisplay.cardData = cardDatas[i];
                cardDisplay.BindCardData();
                cardDisplay.BindCardVisualData();
                cardDisplay.gameObject.SetActive(true);

                Helpers.DelayForSeconds(.3f, () => {
                    Debug.Log("1");
                    cardDisplay.OnClick = () => {
                        WorldSystem.instance.characterManager.AddCardToDeck(cardDisplay);
                        WorldSystem.instance.deckDisplayManager.StartCoroutine(cardDisplay.AnimateCardToDeck());
                        cardPanel.SetActive(false);
                        aReward.RemoveReward();
                    };
                });
            }
        }
    }

    void CreateUnlockCardReward(Reward reward)
    {
        if (Reward.GetCardData(reward.value) is List<CardData> cardDatas)
        {
            cardPanel.SetActive(true);
            foreach (var item in displayCards)
            {
                item.gameObject.SetActive(false);
            }
            for (int i = 0; i < cardDatas.Count; i++)
            {
                if (i >= displayCards.Length) break;

                CardDisplay cardDisplay = displayCards[i];

                cardDisplay.OnClick = null;

                cardDisplay.cardData = cardDatas[i];
                cardDisplay.BindCardData();
                cardDisplay.BindCardVisualData();
                cardDisplay.gameObject.SetActive(true);

                Helpers.DelayForSeconds(.3f, () => {
                    cardDisplay.OnClick = () => {
                        WorldSystem.instance.townManager.scribe.UnlockCard(cardDisplay.cardData);
                        cardPanel.SetActive(false);
                        reward.RemoveReward();
                    };
                });
            }
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

    void AddRewardToScreen(Reward aReward)
    {
        aReward.SetWorldReward();
        aReward.transform.localScale = Vector3.one * 2;
        aReward.transform.localPosition = new Vector3(aReward.transform.localPosition.x, aReward.transform.localPosition.y, 0);
        aReward.GetComponent<Button>().onClick.RemoveAllListeners();

        void RewardCallback()
        {
            aReward.callback?.Invoke();
            aReward.GetComponent<ToolTipScanner>().ExitAction();
            aReward.RemoveReward();
        }

        Helpers.DelayForSeconds(.3f, () => {
            aReward.GetComponent<Button>().onClick.AddListener(() => RewardCallback());
            content.GetComponent<Button>().onClick.AddListener(() => RewardCallback());
        });
    }
}
