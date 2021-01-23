using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardCard : Reward
{
    public GameObject cardDisplayPrefab;
    private List<GameObject> cards = new List<GameObject>();

    void Start()
    {
        foreach (Transform card in WorldSystem.instance.uiManager.rewardScreen.rewardScreenCardContent.transform)
        {
            cards.Add(card.gameObject);
        }
    }
    protected override void CollectCombatReward()
    {
        WorldSystem.instance.uiManager.rewardScreen.currentReward = this.GetComponent<RewardCard>();

        if(cards.Count <  WorldSystem.instance.characterManager.maxCardReward)
        {
            GameObject newCard = Instantiate(cardDisplayPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0)) as GameObject;
            newCard.transform.SetParent(WorldSystem.instance.uiManager.rewardScreen.rewardScreenCardContent.transform);
            cards.Add(newCard);
        }
        else if (cards.Count > WorldSystem.instance.characterManager.maxCardReward)
        {
            while (cards.Count > WorldSystem.instance.characterManager.maxCardReward)
            {
                DestroyImmediate(cards[cards.Count - 1]);
                cards.RemoveAt(cards.Count - 1);
            }
        }

        WorldSystem.instance.uiManager.rewardScreen.rewardScreenCard.SetActive(true);
        foreach (GameObject card in cards)
        {
            card.GetComponent<CardDisplay>().cardData = DatabaseSystem.instance.GetRandomCard();
            card.GetComponent<CardDisplay>().BindCardData();
        }

        // WorldSystem.instance.uiManager.rewardScreen.rewardScreenCardContent
        //WorldSystem.instance.uiManager.rewardScreen.rewardScreenCardContent
        
    }
    

    public List<CardData> ChooseCard(CharacterClass characterClass, int amount = 3, bool random = true, CardData cardData = null)
    {
        List<CardData> cardReward = new List<CardData>(); 

        for (int i = 0; i < amount; i++)
        {
            cardReward.Add(DatabaseSystem.instance.GetRandomCard(characterClass));
        }

        return cardReward;
    }
}
