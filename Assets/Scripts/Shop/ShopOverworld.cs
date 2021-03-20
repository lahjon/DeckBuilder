using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopOverworld : MonoBehaviour
{
    public CardDisplay currentCard;
    public List<GameObject> cardsInStock;
    public List<TMP_Text> cardsPrices;

    void UpdateCardPrices()
    {
        for (int i = 0; i < cardsInStock.Count; i++)
        {
            if(cardsInStock[i].gameObject.activeSelf)
                cardsPrices[i].text = cardsInStock[i].GetComponent<CardVisual>().cardData.goldValue.ToString() + " g";
            else
                cardsPrices[i].text = "Out of Stock!";
        }
    }
    void GetNewCards()
    {
        foreach (GameObject card in cardsInStock)
        {
            card.GetComponent<CardDisplay>().cardData = DatabaseSystem.instance.GetRandomCard();
            card.GetComponent<CardDisplay>().BindCardData();
        }
    }
    public void RestockShop()
    {
        foreach (GameObject card in cardsInStock)
        {
            card.SetActive(true);
        }
        GetNewCards();
        UpdateCardPrices();
    }
    void InsufficientGold()
    {
        Debug.Log("Not enough Gold!");
    }
    public void PurchaseCard(CardVisual clickedCard)
    {
        int characterGold = WorldSystem.instance.characterManager.gold;
        int goldCost = clickedCard.cardData.goldValue;
        if (characterGold >= goldCost)
        {
            WorldSystem.instance.characterManager.gold -= goldCost;
            WorldSystem.instance.characterManager.AddCardDataToDeck(clickedCard.cardData);
            clickedCard.gameObject.SetActive(false);
            clickedCard.ResetScale();
            UpdateCardPrices();
        }
        else
        {
            InsufficientGold();
        }
    }
    public void DebugRemoveCard()
    {
        WorldSystem.instance.characterManager.RemoveCardDataFromDeck(WorldSystem.instance.characterManager.playerCardsData.Count - 1);
    }
    public void DebugAddGold()
    {
        WorldSystem.instance.characterManager.gold += 100;
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateCharacterHUD();
    }

    public void ButtonLeave()
    {
        WorldStateSystem.SetInShop(false);
    }

}
