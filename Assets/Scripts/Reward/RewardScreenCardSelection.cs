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
        Debug.Log("Card reward");
        WorldStateSystem.SetInCombatReward(true);

        foreach (Transform card in WorldSystem.instance.combatRewardManager.rewardScreenCombat.rewardScreenCardContent.transform)
        {
            cards.Add(card.GetComponent<CardDisplay>());
        }

        int maxCardReward;
        if(rewardCards != null && rewardCards.Any())
            maxCardReward = rewardCards.Count;
        else
            maxCardReward = WorldSystem.instance.characterManager.maxCardReward + CharacterStats.DraftAmount.GetStatAndNotify();

        if(cards.Count < maxCardReward)
        {
            CardDisplay newCard = Instantiate(WorldSystem.instance.combatRewardManager.rewardScreenCombat.cardDisplayPrefab, WorldSystem.instance.combatRewardManager.rewardScreenCombat.rewardScreenCardContent.transform).GetComponent<CardDisplay>();
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

        canvas.SetActive(true);
        if(rewardCards != null && rewardCards.Any())
        {
            Debug.Log("Get Specific Cards");
            SetSpecificCard(rewardCards);
        }
        else
        {
            Debug.Log("Get Random Cards");
            SetRandomCards();   
        }
    }
    private void SetRandomCards()
    {
        Debug.Log(cards.Count);
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

