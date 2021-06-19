using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopOverworld : MonoBehaviour
{
    public CardDisplay currentCard;
    public List<CardVisual> cardsInStock;
    public List<TMP_Text> cardsPrices;
    public Canvas canvas;

    void OnEnable()
    {
        canvas.worldCamera = Camera.main;
    }

    void UpdateCardPrices()
    {
        for (int i = 0; i < cardsInStock.Count; i++)
        {
            if(cardsInStock[i].gameObject.activeSelf)
                cardsPrices[i].text = cardsInStock[i].cardData.goldValue.ToString() + " g";
            else
                cardsPrices[i].text = "Out of Stock!";
        }
    }
    void GetNewCards()
    {
        foreach (CardVisual card in cardsInStock)
        {
            card.cardData = DatabaseSystem.instance.GetRandomCard((CardClassType)WorldSystem.instance.characterManager.character.classType);
            card.BindCardData();
            card.BindCardVisualData();
        }
    }
    public void RestockShop()
    {
        foreach (CardDisplay card in cardsInStock)
        {
            card.gameObject.SetActive(true);
        }
        GetNewCards();
        UpdateCardPrices();
    }
    void InsufficientGold()
    {
        Debug.Log("Not enough Gold!");
        // add animatino or something
    }
    public bool PurchaseCard(CardVisual clickedCard)
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
            return true;
        }
        else
        {
            InsufficientGold();
            return false;
        }
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
