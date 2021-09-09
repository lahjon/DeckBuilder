using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CombatDeckDisplay : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform content;
    public Transform deckDisplay;
    public Transform cardParent;
    public GameObject buttonClose, buttonConfirm;

    public TMP_Text selectAmountText;
    public TMP_Text titleText;
    public int selectAmount;
    public List<CardVisual> sourceCards = new List<CardVisual>();
    public List<CardVisual> selectedCards = new List<CardVisual>();
    public List<CardDisplay> allDisplayedCards = new List<CardDisplay>();

    void Open()
    {
        deckDisplay.gameObject.SetActive(true);
        WorldStateSystem.SetInDisplay();
    }

    public void OpenDeckDisplay(CardLocation cardLocation, int aSelectAmount = 0)
    {
        selectAmount = aSelectAmount;
        sourceCards.Clear();
        selectedCards.Clear();

        switch (cardLocation)
        {
            case CardLocation.Deck:
                if (selectAmount > 0) EnableSelect("Select cards from your deck");
                else DisableSelect("Deck");
                CombatSystem.instance.Hero.deck.ForEach(c => sourceCards.Add((CardVisual)c));
                break;
            case CardLocation.Discard:
                if (selectAmount > 0) EnableSelect("Select cards from your discard");
                else DisableSelect("Discard");
                CombatSystem.instance.Hero.discard.ForEach(c => sourceCards.Add((CardVisual)c));
                break;
            case CardLocation.Exhaust:
                if (selectAmount > 0) EnableSelect("Select cards from your exhaust");
                else DisableSelect("Exhaust");
                CombatSystem.instance.Hero.exhaust.ForEach(c => sourceCards.Add((CardVisual)c));
                break;
            default:
                break;
        }
        UpdateAllCards();
        Open();
    }

    void EnableSelect(string aText)
    {
        titleText.text = aText;
        selectAmountText.text = string.Format("{0} / {1}", selectedCards.Count, selectAmount);
        selectAmountText.gameObject.SetActive(true);
        buttonClose.SetActive(false);
        buttonConfirm.SetActive(true);
    }
    void DisableSelect(string aText)
    {
        titleText.text = aText;
        selectAmountText.gameObject.SetActive(false);
        buttonClose.SetActive(true);
        buttonConfirm.SetActive(false);
    }

    public void UpdateAllCards()
    {
        if (sourceCards.Count > allDisplayedCards.Count)
        {
            while (sourceCards.Count > allDisplayedCards.Count)
            {
                CardDisplay newCard = Instantiate(cardPrefab,content.gameObject.transform).GetComponent<CardDisplay>();
                newCard.transform.SetParent(content.gameObject.transform);
                newCard.gameObject.SetActive(true);
                allDisplayedCards.Add(newCard);
            }
        }
        else if(sourceCards.Count < allDisplayedCards.Count)
        {
            while (sourceCards.Count < allDisplayedCards.Count)
            {   
                Destroy(allDisplayedCards[(allDisplayedCards.Count - 1)].gameObject);
                allDisplayedCards.RemoveAt(allDisplayedCards.Count - 1);
            }
        }

        for (int i = 0; i < sourceCards.Count; i++)
        {
            CardDisplay newCard = allDisplayedCards[i];
            newCard.Mimic(sourceCards[i]);
            newCard.selected = false;

            if (selectAmount > 0) 
                newCard.clickCallback = () => newCard.SelectCard();
            else 
                newCard.clickCallback = () => WorldSystem.instance.deckDisplayManager.DisplayCard(newCard);
        }
    }

    public void AddCard(CardDisplay aCard)
    {
        if (sourceCards[allDisplayedCards.IndexOf(aCard)] is CardVisual card)
        {
            selectedCards.Add(card);
            selectAmountText.text = string.Format("{0} / {1}", selectedCards.Count, selectAmount);
        }
    }
    public void RemoveCard(CardDisplay aCard)
    {
        if (sourceCards[allDisplayedCards.IndexOf(aCard)] is CardVisual card)
        {
            selectedCards.Remove(card);
            selectAmountText.text = string.Format("{0} / {1}", selectedCards.Count, selectAmount);
        }
    }

    public void ButtonConfirm()
    {
        if (selectedCards.Count < selectAmount)
        {
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning(string.Format("Select {0} cards!", selectAmount));
            return;
        }
        Debug.Log("Trigger callback from here!");
        ButtonClose();
    }
    public void ButtonClose()
    {
        WorldStateSystem.TriggerClear();
        deckDisplay.gameObject.SetActive(false);
    }
}

// public class DeckDisplayList<T> : List<T>
// {
//     public new void Add(T item)
//     {
//         base.Add(item);
        
//     }
//     public new void Remove(T item)
//     {
//         base.Remove(item);
        
//     }
// }