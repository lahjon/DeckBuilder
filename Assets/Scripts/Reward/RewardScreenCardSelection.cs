using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RewardScreenCardSelection : MonoBehaviour
{
    private List<CardDisplay> cards = new List<CardDisplay>();
    public GameObject canvas;
    public Transform background;

    public void SetupRewards(List<CardData> rewardCards = null)
    {
        cards.Clear();
        WorldStateSystem.SetInCombatReward(true);

        foreach (Transform card in WorldSystem.instance.rewardManager.rewardScreenCombat.rewardScreenCardContent.transform)
        {
            cards.Add(card.GetComponent<CardDisplay>());
        }

        int maxCardReward;
        if(rewardCards != null && rewardCards.Any())
            maxCardReward = rewardCards.Count;
        else
            maxCardReward = WorldSystem.instance.characterManager.maxCardReward;

        Debug.Log(cards.Count);
        Debug.Log(maxCardReward);
        if(cards.Count < maxCardReward)
        {
            CardDisplay newCard = Instantiate(WorldSystem.instance.rewardManager.rewardScreenCombat.cardDisplayPrefab, WorldSystem.instance.rewardManager.rewardScreenCombat.rewardScreenCardContent.transform).GetComponent<CardDisplay>();
            cards.Add(newCard);
        }
        else if (cards.Count > maxCardReward)
        {
            while (cards.Count > maxCardReward)
            {
                Destroy(cards[cards.Count - 1].gameObject);
                cards.RemoveAt(cards.Count - 1);
            }
        }

        WorldSystem.instance.rewardManager.rewardScreenCombat.canvasCard.SetActive(true);
        if(rewardCards != null && rewardCards.Any())
        {
            Debug.Log("Yes");
            SetSpecificCard(rewardCards);
        }
        else
        {
            Debug.Log("No");
            SetRandomCards();   
        }
    }
    private void SetRandomCards()
    {
        foreach (CardDisplay card in cards)
        {
            card.cardData = DatabaseSystem.instance.GetRandomCard((CardClassType)WorldSystem.instance.characterManager.selectedCharacterClassType);
            card.BindCardData();
            card.BindCardVisualData();
            card.gameObject.SetActive(true);
            card.OnClick = () => {
                card.RewardCallback();
            };
        }
    }
    private void SetSpecificCard(List<CardData> rCards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            CardDisplay card = cards[i];
            card.cardData = rCards[i];
            card.BindCardData();
            card.BindCardVisualData();
            card.gameObject.SetActive(true);
            Debug.Log("Get");
            card.OnClick = () => {
                card.RewardCallback();
            };
        }
    }
    
}

