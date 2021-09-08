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

    public TMP_Text titleText;
    public List<CardVisual> sourceCards;
    public List<CardDisplay> allDisplayedCards;

    void Open()
    {
        deckDisplay.gameObject.SetActive(true);
        WorldStateSystem.SetInDisplay();
    }

    public void OpenDiscard()
    {
        UpdateAllCards(CardLocation.Discard);
        titleText.text = "Discard";
        Open();
    }

    public void OpenDeck()
    {
        UpdateAllCards(CardLocation.Deck);
        titleText.text = "Deck";
        Open();
    }

    public void OpenExhaust()
    {
        UpdateAllCards(CardLocation.Exhaust);
        titleText.text = "Exhaust";
        Open();
    }

    public void UpdateAllCards(CardLocation cardLocation)
    {
        sourceCards.Clear();
        switch (cardLocation)
        {
            case CardLocation.Deck:
                CombatSystem.instance.Hero.deck.ForEach(c => sourceCards.Add((CardVisual)c));
                break;
            case CardLocation.Discard:
                CombatSystem.instance.Hero.discard.ForEach(c => sourceCards.Add((CardVisual)c));
                break;
            case CardLocation.Exhaust:
                CombatSystem.instance.Hero.exhaust.ForEach(c => sourceCards.Add((CardVisual)c));
                break;
            default:
                break;
        }

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
            Debug.Log("Add callback");
            newCard.clickCallback = () => WorldSystem.instance.deckDisplayManager.DisplayCard(newCard);
        }
    }

    public void ButtonClose()
    {
        WorldStateSystem.TriggerClear();
        deckDisplay.gameObject.SetActive(false);
    }
}