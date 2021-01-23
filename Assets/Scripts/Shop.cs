using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    public CardDisplay currentCard;
    public List<GameObject> cardsInStock;
    public List<TMP_Text> cardsPrices;

    void Start()
    {
        RestockShop();
    }
    void UpdateCardPrices()
    {
        for (int i = 0; i < cardsInStock.Count; i++)
        {
            if(cardsInStock[i].gameObject.activeSelf)
                cardsPrices[i].text = cardsInStock[i].GetComponent<Card>().cardData.goldValue.ToString() + " g";
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
    public void PurchaseCard(Card clickedCard)
    {
        int characterGold = WorldSystem.instance.characterManager.gold;
        int goldCost = clickedCard.cardData.goldValue;
        if (characterGold >= goldCost)
        {
            WorldSystem.instance.characterManager.gold -= goldCost;
            WorldSystem.instance.characterManager.AddCardDataToDeck(clickedCard.cardData);
            WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
            clickedCard.gameObject.SetActive(false);
            clickedCard.ResetScale();
            UpdateCardPrices();
        }
        else
        {
            InsufficientGold();
        }
    }
    public void LeaveShop()
    {
        Debug.Log("Leave Shop!");
        this.gameObject.SetActive(false);
        WorldSystem.instance.SwapState(WorldState.Overworld);
    }

    public void EnterShop()
    {
        Debug.Log("Enter Shop!");
        this.gameObject.SetActive(true);
        WorldSystem.instance.SwapState(WorldState.Shop);
    }

    public void DebugDisplay()
    {
        WorldSystem.instance.deckDisplayManager.DisplayDeck();
    }
    public void DebugRemoveCard()
    {
        WorldSystem.instance.characterManager.RemoveCardDataFromDeck(WorldSystem.instance.characterManager.playerCardsData.Count - 1);
    }
    public void DebugAddGold()
    {
        WorldSystem.instance.characterManager.gold += 100;
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateUI();
    }
}
