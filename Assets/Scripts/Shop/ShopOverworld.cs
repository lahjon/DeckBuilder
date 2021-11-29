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
            ArtifactData artifactData = WorldSystem.instance.artifactManager.GetRandomAvailableArtifact();
            if (artifactData == null)
            {
                a.gameObject.SetActive(false);
                artifactPrices[artifactsInStock.IndexOf(a)].text = "Out of stock!";
                continue;
            }
            else
                a.BindData(artifactData);
                
            artifactPrices[artifactsInStock.IndexOf(a)].text = artifactsInStock[artifactsInStock.IndexOf(a)].artifactData.goldValue.ToString() + " g";
            a.button.interactable = true;
            a.button.onClick.RemoveAllListeners();
            a.button.onClick.AddListener(() => PurchaseArtifact(a));
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
        int characterGold = WorldSystem.instance.characterManager.characterCurrency.gold;
        int goldCost = artifact.artifactData.goldValue;
        if (characterGold >= goldCost)
        {
            WorldSystem.instance.characterManager.characterCurrency.gold -= goldCost;
            WorldSystem.instance.artifactManager.AddArtifact(artifact.artifactData.itemId);
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
        int characterGold = WorldSystem.instance.characterManager.characterCurrency.gold;
        int goldCost = clickedCard.cardData.goldValue;
        if (characterGold >= goldCost)
        {
            WorldSystem.instance.characterManager.characterCurrency.gold -= goldCost;
            WorldSystem.instance.characterManager.AddCardToDeck(clickedCard);
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
        WorldSystem.instance.characterManager.characterCurrency.gold += 100;
    }

    public void ButtonLeave()
    {
        WorldSystem.instance.artifactManager.allUnavailableArtifactsNames.Clear();
        WorldStateSystem.SetInOverworldShop(false);
    }

}
