using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardScreenCardSelection : MonoBehaviour
{
    private List<GameObject> cards = new List<GameObject>();
    public GameObject canvas;
    public Transform background;

    public void SetupRewards(List<CardData> rewardCards = null)
    {
        cards.Clear();
        WorldStateSystem.SetInReward(true);

        foreach (Transform card in WorldSystem.instance.rewardManager.rewardScreen.rewardScreenCardContent.transform)
        {
            cards.Add(card.gameObject);
        }

        int maxCardReward;
        if(rewardCards != null && rewardCards.Count > 0)
            maxCardReward = rewardCards.Count;
        else
            maxCardReward = WorldSystem.instance.characterManager.maxCardReward;

        if(cards.Count < maxCardReward)
        {
            GameObject newCard = Instantiate(WorldSystem.instance.rewardManager.rewardScreen.cardDisplayPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0)) as GameObject;
            newCard.transform.SetParent(WorldSystem.instance.rewardManager.rewardScreen.rewardScreenCardContent.transform);
            cards.Add(newCard);
        }
        else if (cards.Count > maxCardReward)
        {
            while (cards.Count > maxCardReward)
            {
                Destroy(cards[cards.Count - 1]);
                cards.RemoveAt(cards.Count - 1);
            }
        }

        WorldSystem.instance.rewardManager.rewardScreen.canvasCard.SetActive(true);
        if(rewardCards != null && rewardCards.Count > 0)
        {
            SetSpecificCard(rewardCards);
        }
        else
        {
            SetRandomCards();   
        }
    }
    private void SetRandomCards()
    {
        foreach (GameObject card in cards)
        {
            card.GetComponent<CardDisplay>().cardData = DatabaseSystem.instance.GetRandomCard();
            card.GetComponent<CardDisplay>().BindCardData();
        }
    }
    private void SetSpecificCard(List<CardData> rCards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].GetComponent<CardDisplay>().cardData = rCards[i];
            cards[i].GetComponent<CardDisplay>().BindCardData();
        }
    }
    
}

