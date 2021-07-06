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
    public List<CardData> allCardsData;
    public List<CardVisual> allDisplayedCards;

    public void OpenDiscard()
    {
        deckDisplay.gameObject.SetActive(true);
        UpdateAllCards(DeckType.CombatDiscard);
        WorldStateSystem.SetInDisplay();

        // CombatSystem.instance.Hero.discard.ForEach(x => {
        //     GameObject card = Instantiate(x.gameObject, content);
        //     card.SetActive(true);
        //     card.transform.localScale = Vector3.one;
        //     Debug.Log(card.transform.localScale);
        // });

        titleText.text = "Discard";
    }

    public void OpenDeck()
    {
        deckDisplay.gameObject.SetActive(true);
        UpdateAllCards(DeckType.CombatDeck);
        WorldStateSystem.SetInDisplay();

        // CombatSystem.instance.Hero.deck.ForEach(x => {
        //     GameObject card = Instantiate(x.gameObject, content);
        //     card.SetActive(true);
        //     card.transform.localScale = Vector3.one;
        //     Debug.Log(card.transform.localScale);
        // });

        titleText.text = "Deck";
    }

    public void UpdateAllCards(DeckType type)
    {
        allCardsData.Clear();
        if (type == DeckType.CombatDeck)
            CombatSystem.instance.Hero.deck.ForEach(x => allCardsData.Add(x.cardData));
        else    
            CombatSystem.instance.Hero.discard.ForEach(x => allCardsData.Add(x.cardData));

        if(allCardsData.Count > allDisplayedCards.Count)
        {
            while (allCardsData.Count > allDisplayedCards.Count)
            {
                CardDisplay newCard = Instantiate(cardPrefab,content.gameObject.transform).GetComponent<CardDisplay>();
                newCard.transform.SetParent(content.gameObject.transform);
                newCard.gameObject.SetActive(true);
                allDisplayedCards.Add(newCard);
            }
        }
        else if(allCardsData.Count < allDisplayedCards.Count)
        {
            while (allCardsData.Count < allDisplayedCards.Count)
            {   
                Destroy(allDisplayedCards[(allDisplayedCards.Count - 1)].gameObject);
                allDisplayedCards.RemoveAt(allDisplayedCards.Count - 1);
            }
        }

        for (int i = 0; i < allCardsData.Count; i++)
        {
            allDisplayedCards[i].cardData = allCardsData[i];
            allDisplayedCards[i].BindCardVisualData();
        }
    }

    public void ButtonClose()
    {
        // for (int i = 0; i < content.childCount; i++)
        // {
        //     Destroy(content.GetChild(i).gameObject);
        // }
        WorldStateSystem.TriggerClear();
        deckDisplay.gameObject.SetActive(false);
    }
}