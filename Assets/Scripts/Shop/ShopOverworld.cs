using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopOverworld : MonoBehaviour
{
    public List<CardDisplay> cardsInStock;
    public List<Artifact> artifactsInStock;
    public List<TMP_Text> cardsPrices;
    public List<TMP_Text> artifactPrices;
    void GetNewCards()
    {
        foreach (CardDisplay card in cardsInStock)
        {
            CardClassType classType = (CardClassType)WorldSystem.instance.characterManager.selectedCharacterClassType;
            card.cardData = DatabaseSystem.instance.GetRandomCard(classType);
            cardsPrices[cardsInStock.IndexOf(card)].text = cardsInStock[cardsInStock.IndexOf(card)].cardData.goldValue.ToString() + " g";
            card.BindCardData();
            card.BindCardVisualData();

            card.OnClick = () => {
                if (WorldSystem.instance.shopManager.shop.PurchaseCard(card))
                {
                    WorldSystem.instance.deckDisplayManager.StartCoroutine(card.AnimateCardToDeck());
                }
            };

        }
    }

    void GetNewArtifacts()
    {
        foreach (Artifact a in artifactsInStock)
        {
            a.itemData = WorldSystem.instance.artifactManager.GetRandomAvailableArtifact();
            if (a.itemData == null)
            {
                a.gameObject.SetActive(false);
                artifactPrices[artifactsInStock.IndexOf(a)].text = "Out of stock!";
                continue;
            }
            artifactPrices[artifactsInStock.IndexOf(a)].text = artifactsInStock[artifactsInStock.IndexOf(a)].itemData.goldValue.ToString() + " g";
            a.button.interactable = true;
            a.button.onClick.RemoveAllListeners();
            a.button.onClick.AddListener(() => PurchaseArtifact(a));
            a.BindData();
        }
    }
    public void RestockShop()
    {
        foreach (CardDisplay card in cardsInStock)
        {
            card.gameObject.SetActive(true);
        }
        GetNewCards();
        GetNewArtifacts();
    }
    void InsufficientGold()
    {
        WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Not enough Gold!");
        Debug.Log("Not enough Gold!");
    }

    public bool PurchaseArtifact(Artifact artifact)
    {
        int characterGold = WorldSystem.instance.characterManager.gold;
        int goldCost = artifact.itemData.goldValue;
        if (characterGold >= goldCost)
        {
            WorldSystem.instance.characterManager.gold -= goldCost;
            WorldSystem.instance.artifactManager.AddArtifact(artifact.itemData.name);
            artifact.gameObject.SetActive(false);
            artifactPrices[artifactsInStock.IndexOf(artifact)].text = "Out of stock!";
            return true;
        }
        else
        {
            InsufficientGold();
            return false;
        }
    }
    public bool PurchaseCard(CardDisplay clickedCard)
    {
        int characterGold = WorldSystem.instance.characterManager.gold;
        int goldCost = clickedCard.cardData.goldValue;
        if (characterGold >= goldCost)
        {
            WorldSystem.instance.characterManager.gold -= goldCost;
            WorldSystem.instance.characterManager.AddCardDataToDeck(clickedCard.cardData);
            clickedCard.gameObject.SetActive(false);
            clickedCard.ResetScale();
            cardsPrices[cardsInStock.IndexOf(clickedCard)].text = "Out of stock!";
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
        WorldSystem.instance.artifactManager.allUnavailableArtifactsNames.Clear();
        WorldStateSystem.SetInOverworldShop(false);
    }

}
